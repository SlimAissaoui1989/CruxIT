using CruxIT.Library.DataToolkit.RepositoryPattern;
using CruxIT.Library.DataToolkit.Tests.Fixtures;
using CruxIT.Library.DataToolkit.Tests.TestData.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CruxIT.Library.DataToolkit.Tests.RepositoryPattern;

public sealed class UnitOfWorkTests
{
    [Fact]
    public async Task SaveChangesAsync_PersistsChangesAndAwaitsEventHandlers()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var unitOfWork = new UnitOfWork<TestData.TestDbContext>(context);
        context.Records.Add(new TestRecord { Name = "Saved" });
        var eventCount = 0;
        unitOfWork.SavedChangesAsync += async (_, _) =>
        {
            await Task.Yield();
            eventCount++;
        };

        var savedCount = await unitOfWork.SaveChangesAsync();

        Assert.Equal(1, savedCount);
        Assert.Equal(1, eventCount);
        Assert.Equal(1, await context.Records.CountAsync());
    }

    [Fact]
    public async Task SaveChangesAsync_PropagatesEventHandlerExceptions()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var unitOfWork = new UnitOfWork<TestData.TestDbContext>(context);
        unitOfWork.SavedChangesAsync += (_, _) => Task.FromException(new InvalidOperationException("Handler failed."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => unitOfWork.SaveChangesAsync());

        Assert.Equal("Handler failed.", exception.Message);
    }

    [Fact]
    public async Task BeginTransactionAsync_ExposesAndClearsCurrentTransaction()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var unitOfWork = new UnitOfWork<TestData.TestDbContext>(context);

        await using (var transaction = await unitOfWork.BeginTransactionAsync())
        {
            Assert.Same(transaction, unitOfWork.CurrentTransaction);
            await transaction.RollbackAsync();
        }

        Assert.Null(unitOfWork.CurrentTransaction);
    }

    [Fact]
    public async Task BeginTransactionAsync_WithIsolationLevel_UsesRequestedLevel()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var unitOfWork = new UnitOfWork<TestData.TestDbContext>(context);

        await using var transaction = await unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

        Assert.NotNull(unitOfWork.CurrentTransaction);
        Assert.Equal(System.Data.IsolationLevel.Serializable, transaction.GetDbTransaction().IsolationLevel);
        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task Set_ReturnsContextDbSet()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var unitOfWork = new UnitOfWork<TestData.TestDbContext>(context);

        Assert.Same(context.Records, unitOfWork.Set<TestRecord>());
    }

    [Fact]
    public async Task DisposeAsync_IsIdempotentAndPreventsFurtherSaves()
    {
        await using var database = await TestDatabase.CreateAsync();
        var context = database.CreateContext();
        var unitOfWork = new UnitOfWork<TestData.TestDbContext>(context);

        await unitOfWork.DisposeAsync();
        await unitOfWork.DisposeAsync();

        await Assert.ThrowsAsync<ObjectDisposedException>(() => unitOfWork.SaveChangesAsync());
    }

    [Fact]
    public async Task Dispose_IsIdempotentAndPreventsFurtherSaves()
    {
        await using var database = await TestDatabase.CreateAsync();
        var context = database.CreateContext();
        var unitOfWork = new UnitOfWork<TestData.TestDbContext>(context);

        unitOfWork.Dispose();
        unitOfWork.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(() => unitOfWork.SaveChangesAsync());
    }
}
