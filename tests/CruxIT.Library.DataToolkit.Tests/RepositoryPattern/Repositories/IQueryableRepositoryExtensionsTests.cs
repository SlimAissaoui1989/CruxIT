using CruxIT.Library.DataToolkit.RepositoryPattern.Repositories;
using CruxIT.Library.DataToolkit.Tests.Fixtures;
using CruxIT.Library.DataToolkit.Tests.TestData.Entities;
using Microsoft.EntityFrameworkCore;

namespace CruxIT.Library.DataToolkit.Tests.RepositoryPattern.Repositories;

public sealed class IQueryableRepositoryExtensionsTests
{
    [Fact]
    public async Task WhereOrderBySkipAndTake_ComposeQuery()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        context.Records.AddRange(
            new TestRecord { Name = "Ada", Amount = 1m },
            new TestRecord { Name = "Grace", Amount = 2m },
            new TestRecord { Name = "Linus", Amount = 3m },
            new TestRecord { Name = "Alan", Amount = 4m });
        await context.SaveChangesAsync();

        var repository = database.CreateQueryableRepository(context);
        var result = await repository
            .Where(record => record.Name != "Ada")
            .OrderByDescending(record => record.Name)
            .Skip(1)
            .Take(1)
            .GetFirstAsync(null);

        Assert.Equal("Grace", result.Name);
    }

    [Fact]
    public async Task OrderByAndDistinctBySelector_ReturnExpectedResults()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        context.Records.AddRange(
            new TestRecord { Name = "Ada", Amount = 1m },
            new TestRecord { Name = "Ada", Amount = 2m },
            new TestRecord { Name = "Grace", Amount = 3m });
        await context.SaveChangesAsync();

        var repository = database.CreateQueryableRepository(context);
        var ordered = await repository.OrderBy(record => record.Name).GetAllAsync();
        var distinctNames = await repository.Distinct(record => record.Name).GetAllAsync();
        var distinctEntities = await repository.Distinct().GetAllAsync();

        Assert.Equal(new[] { "Ada", "Ada", "Grace" }, ordered.Select(record => record.Name));
        Assert.Equal(2, distinctNames.Count);
        Assert.Equal(3, distinctEntities.Count);
        Assert.Contains(distinctNames, record => record.Name == "Ada");
        Assert.Contains(distinctNames, record => record.Name == "Grace");
    }

    [Fact]
    public async Task AsNoTrackingAndAsTracking_ConfigureEntityTracking()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var entity = new TestRecord { Name = "Ada", Amount = 1m };
        context.Records.Add(entity);
        await context.SaveChangesAsync();
        var entityId = entity.Id;
        context.ChangeTracker.Clear();

        var repository = database.CreateQueryableRepository(context);
        var noTrackingEntity = await repository.AsNoTracking().GetByIdAsync(entityId);
        Assert.Equal(EntityState.Detached, context.Entry(noTrackingEntity).State);

        var trackingEntity = await repository.AsTracking().GetByIdAsync(entityId);
        Assert.Equal(EntityState.Unchanged, context.Entry(trackingEntity).State);
    }
}
