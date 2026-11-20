using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;
using CruxIT.Library.DataToolkit.RepositoryPattern;
using CruxIT.Library.DataToolkit.RepositoryPattern.Repositories;
using CruxIT.Library.DataToolkit.Tests.TestData;

namespace CruxIT.Library.DataToolkit.Tests.Fixtures;

internal sealed class TestRepository<TEntity> : Repository<UnitOfWork<TestDbContext>, int, TEntity>
    where TEntity : class, IEntityBase<int>
{
    public TestRepository(UnitOfWork<TestDbContext> unitOfWork)
        : base(unitOfWork)
    {
        UseTrace = false;
    }
}
