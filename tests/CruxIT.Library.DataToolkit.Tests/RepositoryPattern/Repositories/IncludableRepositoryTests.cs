using CruxIT.Library.DataToolkit.RepositoryPattern.Repositories;
using CruxIT.Library.DataToolkit.Tests.Fixtures;
using CruxIT.Library.DataToolkit.Tests.TestData;
using CruxIT.Library.DataToolkit.Tests.TestData.Entities;

namespace CruxIT.Library.DataToolkit.Tests.RepositoryPattern.Repositories;

public sealed class IncludableRepositoryTests
{
    [Fact]
    public async Task Include_ThenInclude_LoadsNestedNavigationProperties()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using (var seedContext = database.CreateContext())
        {
            seedContext.Records.Add(new TestRecord
            {
                Name = "Parent",
                Children =
                [
                    new TestChild
                    {
                        Detail = new TestDetail { Description = "Nested detail" }
                    }
                ]
            });
            await seedContext.SaveChangesAsync();
        }

        await using var queryContext = database.CreateContext();
        var repository = database.CreateQueryableRepository(queryContext);
        var record = await repository
            .Include(item => item.Children)
            .ThenInclude(child => child.Detail)
            .GetFirstAsync(item => item.Name == "Parent");

        var child = Assert.Single(record.Children);
        Assert.NotNull(child.Detail);
        Assert.Equal("Nested detail", child.Detail.Description);
    }

    [Fact]
    public async Task IncludeReference_ThenIncludeReference_LoadsNestedReference()
    {
        await using var database = await TestDatabase.CreateAsync();
        int childId;
        await using (var seedContext = database.CreateContext())
        {
            var record = new TestRecord
            {
                Name = "Parent",
                Children =
                [
                    new TestChild
                    {
                        Detail = new TestDetail
                        {
                            Description = "Detail",
                            Metadata = new TestMetadata { Value = "Nested metadata" }
                        }
                    }
                ]
            };
            seedContext.Records.Add(record);
            await seedContext.SaveChangesAsync();
            childId = record.Children.Single().Id;
        }

        await using var queryContext = database.CreateContext();
        var repository = database.CreateQueryableRepository<TestChild>(queryContext);
        var result = await repository
            .Include(child => child.Detail!)
            .ThenInclude(detail => detail.Metadata)
            .GetByIdAsync(childId);

        Assert.NotNull(result.Detail);
        Assert.NotNull(result.Detail.Metadata);
        Assert.Equal("Nested metadata", result.Detail.Metadata.Value);
    }
}
