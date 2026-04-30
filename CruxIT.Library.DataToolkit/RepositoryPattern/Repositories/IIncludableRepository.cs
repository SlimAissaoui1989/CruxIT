using CruxIT.Library.DataToolkit.EntityFramework;
using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.RepositoryPattern.Repositories
{
    public interface IIncludableRepository<TUnitOfWork, TKey, TEntity, out TProperty> : IQueryableRepository<TUnitOfWork, TKey, TEntity>
            where TUnitOfWork : IUnitOfWork
            where TEntity : class, IEntityBase<TKey>
            where TKey : IEquatable<TKey>
    {
        new IIncludableQueryable<TEntity, TProperty> Entities { get; }
    }
}
