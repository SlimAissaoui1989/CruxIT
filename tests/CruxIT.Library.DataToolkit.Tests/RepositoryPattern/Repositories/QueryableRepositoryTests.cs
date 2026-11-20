using AutoMapper;
using CruxIT.Library.Exceptions;
using CruxIT.Library.DataToolkit.RepositoryPattern;
using CruxIT.Library.DataToolkit.RepositoryPattern.Repositories;
using CruxIT.Library.DataToolkit.Tests.Fixtures;
using CruxIT.Library.DataToolkit.Tests.TestData;
using CruxIT.Library.DataToolkit.Tests.TestData.Entities;
using CruxIT.Library.DataToolkit.Tests.TestData.Projections;
using Microsoft.Extensions.Logging.Abstractions;

namespace CruxIT.Library.DataToolkit.Tests.RepositoryPattern.Repositories;

public sealed class QueryableRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_AppliesPredicateAndSoftDeleteFilter()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        context.Records.AddRange(
            new TestRecord { Name = "Ada", Amount = 10m },
            new TestRecord { Name = "Alan", Amount = 20m },
            new TestRecord { Name = "Ada deleted", Amount = 100m, IsDeleted = true },
            new TestRecord { Name = "Grace", Amount = 30m });
        await context.SaveChangesAsync();

        var repository = database.CreateQueryableRepository(context);
        var results = await repository.GetAllAsync(record => record.Name.StartsWith("A"));

        Assert.Equal(new[] { "Ada", "Alan" }, results.Select(record => record.Name).OrderBy(name => name));
    }

    [Fact]
    public async Task CountAsyncAndAnyAsync_ApplyPredicate()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        context.Records.AddRange(
            new TestRecord { Name = "Ada", Amount = 10m },
            new TestRecord { Name = "Alan", Amount = 20m },
            new TestRecord { Name = "Ada deleted", Amount = 100m, IsDeleted = true },
            new TestRecord { Name = "Grace", Amount = 30m });
        await context.SaveChangesAsync();

        var repository = database.CreateQueryableRepository(context);

        Assert.Equal(2, await repository.CountAsync(record => record.Name.StartsWith("A")));
        Assert.True(await repository.AnyAsync(record => record.Name == "Grace"));
        Assert.False(await repository.AnyAsync(record => record.Name == "Missing"));
    }

    [Fact]
    public async Task Getters_ReturnMatchingEntitiesOrNotFoundResults()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var entity = new TestRecord { Name = "Ada", Amount = 10m };
        context.Records.Add(entity);
        await context.SaveChangesAsync();

        var repository = database.CreateQueryableRepository(context);

        Assert.Equal(entity.Id, (await repository.GetFirstOrDefaultAsync())!.Id);
        Assert.Equal(entity.Id, (await repository.GetFirstAsync(record => record.Name == "Ada")).Id);
        Assert.Equal(entity.Id, (await repository.GetByIdAsync(entity.Id)).Id);
        Assert.Equal(entity.Id, (await repository.GetByIdOrDefaultAsync(entity.Id))!.Id);
        Assert.Null(await repository.GetFirstOrDefaultAsync(record => record.Name == "Missing"));
        Assert.Null(await repository.GetByIdOrDefaultAsync(-1));

        var exception = await Assert.ThrowsAsync<CxException>(() => repository.GetByIdAsync(-1));
        Assert.Equal(CxHttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task ProjectionMethods_ProjectToDtoAndRequireMapper()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        context.Records.AddRange(
            new TestRecord { Name = "Ada", Amount = 10m },
            new TestRecord { Name = "Grace", Amount = 20m });
        await context.SaveChangesAsync();

        var withoutMapper = database.CreateQueryableRepository(context);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => withoutMapper.GetAllAsync<TestRecordSummary>());

        var configuration = new MapperConfiguration(
            cfg => cfg.CreateMap<TestRecord, TestRecordSummary>(),
            NullLoggerFactory.Instance);
        var mapper = configuration.CreateMapper();
        var unitOfWork = new UnitOfWork<TestDbContext>(context, mapper);
        var repository = new QueryableRepository<UnitOfWork<TestDbContext>, int, TestRecord>(unitOfWork);

        var projected = await repository.GetAllAsync<TestRecordSummary>(record => record.Name == "Ada");
        var first = await repository.GetByIdAsync<TestRecordSummary>(
            (await repository.GetFirstAsync(record => record.Name == "Ada")).Id);
        var optionalFirst = await repository.GetFirstOrDefaultAsync<TestRecordSummary>(record => record.Name == "Ada");
        var requiredFirst = await repository.GetFirstAsync<TestRecordSummary>(record => record.Name == "Ada");
        var optionalById = await repository.GetByIdOrDefaultAsync<TestRecordSummary>(first.Id);

        var result = Assert.Single(projected);
        Assert.Equal("Ada", result.Name);
        Assert.Equal(result.Id, first.Id);
        Assert.Equal(result.Id, optionalFirst!.Id);
        Assert.Equal(result.Id, requiredFirst.Id);
        Assert.Equal(result.Id, optionalById!.Id);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsPageAndFilteredTotal()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        context.Records.AddRange(
            new TestRecord { Name = "Ada", Amount = 10m },
            new TestRecord { Name = "Alan", Amount = 20m },
            new TestRecord { Name = "Grace", Amount = 30m },
            new TestRecord { Name = "Linus", Amount = 40m },
            new TestRecord { Name = "Deleted", Amount = 50m, IsDeleted = true });
        await context.SaveChangesAsync();

        var repository = database.CreateQueryableRepository(context);
        var page = await repository.GetPagedAsync(2, 2);

        Assert.Equal(2, page.Items.Count);
        Assert.Equal(4, page.TotalCount);
    }

    [Fact]
    public async Task Aggregates_IgnoreSoftDeletedRecords()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        context.Records.AddRange(
            new TestRecord { Name = "First", Amount = 10m, IntegerAmount = 1, NullableIntegerAmount = 1, LongAmount = 1, NullableLongAmount = 1, FloatAmount = 1, NullableFloatAmount = 1, DoubleAmount = 1, NullableDoubleAmount = 1, NullableAmount = 10m },
            new TestRecord { Name = "Second", Amount = 20m, IntegerAmount = 2, NullableIntegerAmount = 2, LongAmount = 2, NullableLongAmount = 2, FloatAmount = 2, NullableFloatAmount = 2, DoubleAmount = 2, NullableDoubleAmount = 2, NullableAmount = 20m },
            new TestRecord { Name = "Deleted", Amount = 100m, IntegerAmount = 100, NullableIntegerAmount = 100, LongAmount = 100, NullableLongAmount = 100, FloatAmount = 100, NullableFloatAmount = 100, DoubleAmount = 100, NullableDoubleAmount = 100, NullableAmount = 100m, IsDeleted = true });
        await context.SaveChangesAsync();

        var repository = database.CreateQueryableRepository(context);

        Assert.Equal(30m, await repository.SumAsync(record => record.Amount));
        Assert.Equal(3, await repository.SumAsync(record => record.IntegerAmount));
        Assert.Equal(3, await repository.SumAsync(record => record.NullableIntegerAmount));
        Assert.Equal(3L, await repository.SumAsync(record => record.LongAmount));
        Assert.Equal(3L, await repository.SumAsync(record => record.NullableLongAmount));
        Assert.Equal(3f, await repository.SumAsync(record => record.FloatAmount));
        Assert.Equal(3f, await repository.SumAsync(record => record.NullableFloatAmount));
        Assert.Equal(3d, await repository.SumAsync(record => record.DoubleAmount));
        Assert.Equal(3d, await repository.SumAsync(record => record.NullableDoubleAmount));
        Assert.Equal(30m, await repository.SumAsync(record => record.NullableAmount));
        Assert.Equal(10m, await repository.SumAsync(record => record.Amount, record => record.Name == "First"));
        Assert.Equal(10m, await repository.MinAsync(record => record.Amount));
        Assert.Equal(20m, await repository.MaxAsync(record => record.Amount));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    public async Task GetPagedAsync_RejectsNonPositivePageArguments(int pageNumber, int pageSize)
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var repository = database.CreateQueryableRepository(context);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => repository.GetPagedAsync(pageNumber, pageSize));
    }
}
