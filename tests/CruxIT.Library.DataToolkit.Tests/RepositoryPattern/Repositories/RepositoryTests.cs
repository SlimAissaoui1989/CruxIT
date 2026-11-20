using CruxIT.Library.DataToolkit.Tests.Fixtures;
using CruxIT.Library.DataToolkit.Tests.TestData.Entities;
using Microsoft.EntityFrameworkCore;

namespace CruxIT.Library.DataToolkit.Tests.RepositoryPattern.Repositories;

public sealed class RepositoryTests
{
    [Fact]
    public async Task AddAsync_SetsCreationAuditFields()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var repository = database.CreateRepository<TestRecord>(context);
        var userId = Guid.NewGuid();
        repository.UnitOfWork.CurrentUserId = userId;
        repository.UnitOfWork.CurrentUserName = "Ada";
        var record = new TestRecord { Name = "Created" };
        var before = DateTime.UtcNow;

        await repository.AddAsync(record);
        await repository.UnitOfWork.SaveChangesAsync();

        Assert.Equal(userId, record.CreatedUserId);
        Assert.Equal("Ada", record.CreatedUserName);
        Assert.InRange(record.CreatedDate, before, DateTime.UtcNow);
        Assert.True(record.Id > 0);
    }

    [Fact]
    public async Task UpdateAsync_SetsUpdateAuditFieldsForDetachedEntity()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var repository = database.CreateRepository<TestRecord>(context);
        var record = new TestRecord { Name = "Before" };
        await repository.AddAsync(record);
        await repository.UnitOfWork.SaveChangesAsync();
        var id = record.Id;
        context.ChangeTracker.Clear();

        var userId = Guid.NewGuid();
        repository.UnitOfWork.CurrentUserId = userId;
        repository.UnitOfWork.CurrentUserName = "Grace";
        var updated = await repository.UpdateAsync(new TestRecord { Id = id, Name = "After" });
        await repository.UnitOfWork.SaveChangesAsync();

        Assert.Equal("After", updated.Name);
        Assert.Equal(userId, updated.LastUpdatedUserId);
        Assert.Equal("Grace", updated.LastUpdatedUserName);
        Assert.NotNull(updated.LastUpdatedDate);
        Assert.Equal("After", (await context.Records.SingleAsync(item => item.Id == id)).Name);
    }

    [Fact]
    public async Task RemoveAsync_SoftDeletesAndSetsDeletionAuditFields()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var record = new TestRecord { Name = "To delete" };
        context.Records.Add(record);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var repository = database.CreateRepository<TestRecord>(context);
        var userId = Guid.NewGuid();
        repository.UnitOfWork.CurrentUserId = userId;
        repository.UnitOfWork.CurrentUserName = "Linus";
        await repository.RemoveAsync(new TestRecord { Id = record.Id, Name = record.Name });
        await repository.UnitOfWork.SaveChangesAsync();

        Assert.Empty(await repository.GetAllAsync());
        var deleted = await context.Records.IgnoreQueryFilters().SingleAsync(item => item.Id == record.Id);
        Assert.True(deleted.IsDeleted);
        Assert.Equal(userId, deleted.DeletedUserId);
        Assert.Equal("Linus", deleted.DeletedUserName);
        Assert.NotNull(deleted.DeletedDate);
    }

    [Fact]
    public async Task RemoveAsync_HardDeletesEntitiesWithoutSoftDeleteSupport()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var record = new TestPlainRecord { Name = "Permanent delete" };
        context.PlainRecords.Add(record);
        await context.SaveChangesAsync();

        var repository = database.CreateRepository<TestPlainRecord>(context);
        await repository.RemoveAsync(record);
        await repository.UnitOfWork.SaveChangesAsync();

        Assert.False(await context.PlainRecords.AnyAsync(item => item.Id == record.Id));
    }

    [Fact]
    public async Task RangeMethods_AddUpdateAndRemoveMultipleEntities()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var repository = database.CreateRepository<TestRecord>(context);
        var records = new[]
        {
            new TestRecord { Name = "First" },
            new TestRecord { Name = "Second" }
        };

        var added = await repository.AddRangeAsync(records);
        await repository.UnitOfWork.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var updates = records.Select(record => new TestRecord { Id = record.Id, Name = $"Updated {record.Name}" }).ToArray();
        var updated = await repository.UpdateRangeAsync(updates);
        await repository.UnitOfWork.SaveChangesAsync();
        await repository.RemoveRangeAsync(CancellationToken.None, updates);
        await repository.UnitOfWork.SaveChangesAsync();

        Assert.Equal(2, added.Count());
        Assert.Equal(2, updated.Count());
        Assert.Empty(await repository.GetAllAsync());
        Assert.Equal(2, await context.Records.IgnoreQueryFilters().CountAsync(record => record.IsDeleted));
    }
}
