using CruxIT.Library.DataToolkit.RepositoryPattern;
using CruxIT.Library.DataToolkit.RepositoryPattern.Repositories;
using CruxIT.Library.DataToolkit.Tests.TestData;
using CruxIT.Library.DataToolkit.Tests.TestData.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CruxIT.Library.DataToolkit.Tests.Fixtures;

internal sealed class TestDatabase : IAsyncDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<TestDbContext> _options;

    private TestDatabase(SqliteConnection connection, DbContextOptions<TestDbContext> options)
    {
        _connection = connection;
        _options = options;
    }

    public static async Task<TestDatabase> CreateAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;
        var database = new TestDatabase(connection, options);

        await using var context = database.CreateContext();
        await context.Database.EnsureCreatedAsync();

        return database;
    }

    public TestDbContext CreateContext() => new(_options);

    public QueryableRepository<UnitOfWork<TestDbContext>, int, TestRecord> CreateQueryableRepository(TestDbContext context)
    {
        var unitOfWork = new UnitOfWork<TestDbContext>(context);
        return new QueryableRepository<UnitOfWork<TestDbContext>, int, TestRecord>(unitOfWork);
    }

    public QueryableRepository<UnitOfWork<TestDbContext>, int, TEntity> CreateQueryableRepository<TEntity>(TestDbContext context)
        where TEntity : class, CruxIT.Library.DataToolkit.EntityFramework.BaseTypes.IEntityBase<int>
    {
        var unitOfWork = new UnitOfWork<TestDbContext>(context);
        return new QueryableRepository<UnitOfWork<TestDbContext>, int, TEntity>(unitOfWork);
    }

    public TestRepository<TEntity> CreateRepository<TEntity>(TestDbContext context)
        where TEntity : class, CruxIT.Library.DataToolkit.EntityFramework.BaseTypes.IEntityBase<int>
    {
        return new TestRepository<TEntity>(new UnitOfWork<TestDbContext>(context));
    }

    public ValueTask DisposeAsync() => _connection.DisposeAsync();
}
