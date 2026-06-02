using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ReqFlow.Domain;
using ReqFlow.Infrastructure;

namespace ReqFlow.Tests;

public sealed class InfrastructureMappingTests
{
    [Fact]
    public void StatusHistoryId_IsClientGenerated()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ReqFlow;Trusted_Connection=True")
            .Options;
        using var dbContext = new AppDbContext(options);

        var property = dbContext.Model.FindEntityType(typeof(RequestStatusHistory))!
            .FindProperty(nameof(RequestStatusHistory.Id));

        Assert.Equal(ValueGenerated.Never, property!.ValueGenerated);
    }
}
