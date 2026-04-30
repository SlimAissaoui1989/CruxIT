using CruxIT.Library.DataToolkit.EntityFramework;
using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;
using CruxIT.Library.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.RepositoryPattern.Repositories
{
    public interface IRepository<TUnitOfWork, TKey, TEntity>  : IQueryableRepository<TUnitOfWork, TKey, TEntity>
        where TUnitOfWork : IUnitOfWork
        where TEntity : class, IEntityBase<TKey>
        where TKey : IEquatable<TKey>
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities);
        Task RemoveAsync(TEntity entity);
        Task RemoveRangeAsync(params TEntity[] entities);
    }
}
