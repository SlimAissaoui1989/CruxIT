using CruxIT.Library.DataToolkit.EntityFramework.Entities;
using CruxIT.Library.DataToolkit.Tests.Fixtures;
using CruxIT.Library.DataToolkit.Tests.TestData;
using CruxIT.Library.DataToolkit.Tests.TestData.Entities;
using Microsoft.EntityFrameworkCore;

namespace CruxIT.Library.DataToolkit.Tests.EntityFramework;

public sealed class CxDbContextTests
{
    [Fact]
    public async Task DateOnlyAndTimeOnlyConventions_RoundTripValues()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var expectedDate = new DateOnly(2026, 11, 20);
        var expectedTime = new TimeOnly(14, 35, 42);
        var record = new TestDateTimeRecord { Date = expectedDate, Time = expectedTime };
        context.DateTimeRecords.Add(record);
        await context.SaveChangesAsync();
        var id = record.Id;
        context.ChangeTracker.Clear();

        var loaded = await context.DateTimeRecords.SingleAsync(item => item.Id == id);

        Assert.Equal(expectedDate, loaded.Date);
        Assert.Equal(expectedTime, loaded.Time);
    }

    [Fact]
    public async Task ForeignKeys_UseRestrictDeleteBehavior()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();

        var foreignKey = context.Model
            .FindEntityType(typeof(TestChild))!
            .GetForeignKeys()
            .Single(key => key.PrincipalEntityType.ClrType == typeof(TestRecord));

        Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior);
    }

    [Fact]
    public async Task TraceConfiguration_CreatesTableNameAndTableIdIndex()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();

        var index = context.Model
            .FindEntityType(typeof(Trace))!
            .GetIndexes()
            .Single(candidate => candidate.Properties.Select(property => property.Name)
                .SequenceEqual([nameof(Trace.TableName), nameof(Trace.TableId)]));

        Assert.Equal(2, index.Properties.Count);
    }
}
