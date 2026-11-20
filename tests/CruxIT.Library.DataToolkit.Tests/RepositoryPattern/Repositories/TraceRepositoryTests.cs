using CruxIT.Library.DataToolkit.EntityFramework.Entities;
using CruxIT.Library.DataToolkit.EntityFramework.Enums;
using CruxIT.Library.DataToolkit.RepositoryPattern;
using CruxIT.Library.DataToolkit.RepositoryPattern.Repositories;
using CruxIT.Library.DataToolkit.Tests.Fixtures;
using CruxIT.Library.DataToolkit.Tests.TestData.Entities;
using Microsoft.EntityFrameworkCore;

namespace CruxIT.Library.DataToolkit.Tests.RepositoryPattern.Repositories;

public sealed class TraceRepositoryTests
{
    [Fact]
    public async Task AddAsync_WithCurrentUser_RecordsEntityChange()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var unitOfWork = new UnitOfWork<TestData.TestDbContext>(context)
        {
            CurrentUserId = Guid.NewGuid(),
            CurrentUserName = "Ada"
        };
        var repository = new Repository<UnitOfWork<TestData.TestDbContext>, int, TestRecord>(unitOfWork);

        await repository.AddAsync(new TestRecord { Name = "Traced entity" });
        await unitOfWork.SaveChangesAsync();

        var trace = await context.Traces.SingleAsync();
        Assert.Equal(nameof(TestRecord), trace.TableName);
        Assert.Equal(unitOfWork.CurrentUserId, trace.UserId);
        Assert.Equal("Ada", trace.UserName);
        Assert.Equal(ActionTypes.Add, trace.ActionType);
        Assert.Contains("Traced entity", trace.Value);
    }

    [Fact]
    public async Task GetEntityTraceAsync_FiltersAndOrdersByActionDate()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var first = CreateTrace("TestRecord", "42", new DateTime(2026, 1, 1));
        var second = CreateTrace("TestRecord", "42", new DateTime(2026, 1, 2));
        var unrelated = CreateTrace("OtherRecord", "42", new DateTime(2025, 1, 1));
        context.Traces.AddRange(second, unrelated, first);
        await context.SaveChangesAsync();
        var unitOfWork = new UnitOfWork<TestData.TestDbContext>(context);

        var traceRepository = Assert.IsType<TraceRepository>(unitOfWork.Trace);
        var traces = await traceRepository.GetEntityTraceAsync("TestRecord", "42");

        Assert.Equal(new[] { first.ActionDate, second.ActionDate }, traces.Select(trace => trace.ActionDate));
    }

    private static Trace CreateTrace(string tableName, string tableId, DateTime actionDate)
    {
        return new Trace
        {
            TableName = tableName,
            TableId = tableId,
            UserId = Guid.NewGuid(),
            UserName = "Test user",
            ActionDate = actionDate,
            ActionType = ActionTypes.Add,
            Value = "{}"
        };
    }
}
