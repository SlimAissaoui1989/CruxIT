using CruxIT.Library.DataToolkit.EntityFramework;
using CruxIT.Library.DataToolkit.Tests.TestData.Entities;
using Microsoft.EntityFrameworkCore;

namespace CruxIT.Library.DataToolkit.Tests.TestData;

internal sealed class TestDbContext : CxDbContext
{
    public TestDbContext() { }

    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options) { }

    public DbSet<TestRecord> Records => Set<TestRecord>();
    public DbSet<TestPlainRecord> PlainRecords => Set<TestPlainRecord>();
    public DbSet<TestDateTimeRecord> DateTimeRecords => Set<TestDateTimeRecord>();
    public DbSet<TestChild> Children => Set<TestChild>();
}
