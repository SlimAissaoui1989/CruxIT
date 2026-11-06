using CruxIT.Library.DataToolkit.EntityFramework;
using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;
using CruxIT.Library.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.RepositoryPattern.Repositories
{
    public interface IQueryableRepository<TUnitOfWork, TKey, TEntity>
        where TUnitOfWork : IUnitOfWork
        where TEntity : class, IEntityBase<TKey>
        where TKey : IEquatable<TKey>
    {
        IQueryable<TEntity> Entities { get; }
        TUnitOfWork UnitOfWork { get; }
        Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default);
        Task<TResult?> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default);
        Task<TResult> GetFirstAsync<TResult>(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default);
        Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
        Task<TResult> GetByIdAsync<TResult>(TKey id, CancellationToken cancellationToken = default);
        Task<TEntity?> GetByIdOrDefaultAsync(TKey id, CancellationToken cancellationToken = default);
        Task<TResult?> GetByIdOrDefaultAsync<TResult>(TKey id, CancellationToken cancellationToken = default);
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        CxException NotFoundByIdException(TKey id);
        CxException EntityNotFoundException();
    }
}
