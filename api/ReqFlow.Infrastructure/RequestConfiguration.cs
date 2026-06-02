using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReqFlow.Domain;

namespace ReqFlow.Infrastructure;

public sealed class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.ToTable("Requests");
        builder.HasKey(request => request.Id);
        builder.Property(request => request.Title).HasMaxLength(150).IsRequired();
        builder.Property(request => request.Description).HasMaxLength(1000).IsRequired();
        builder.Property(request => request.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(request => request.RequestedBy).HasMaxLength(100).IsRequired();
        builder.Property(request => request.ApprovedRejectedBy).HasMaxLength(100);
        builder.Property(request => request.RejectionReason).HasMaxLength(500);
        builder.Property(request => request.RowVersion).IsRowVersion();
        builder.HasIndex(request => request.Status);
        builder.HasIndex(request => request.CreatedAt);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(request => request.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(request => request.ApprovedRejectedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(request => request.StatusHistory)
            .WithOne()
            .HasForeignKey(history => history.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(request => request.StatusHistory)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
