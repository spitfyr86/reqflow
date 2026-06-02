using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReqFlow.Domain;

namespace ReqFlow.Infrastructure;

public sealed class RequestStatusHistoryConfiguration : IEntityTypeConfiguration<RequestStatusHistory>
{
    public void Configure(EntityTypeBuilder<RequestStatusHistory> builder)
    {
        builder.ToTable("RequestStatusHistory");
        builder.HasKey(history => history.Id);
        builder.Property(history => history.FromStatus).HasConversion<string>().HasMaxLength(30);
        builder.Property(history => history.ToStatus).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(history => history.ChangedBy).HasMaxLength(100).IsRequired();
        builder.Property(history => history.Comment).HasMaxLength(500);
        builder.HasIndex(history => history.RequestId);
    }
}
