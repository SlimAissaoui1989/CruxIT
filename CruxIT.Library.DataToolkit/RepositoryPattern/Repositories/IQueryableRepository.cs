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
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<(List<TEntity> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<int> SumAsync(Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<int?> SumAsync(Expression<Func<TEntity, int?>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<long> SumAsync(Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<long?> SumAsync(Expression<Func<TEntity, long?>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<float> SumAsync(Expression<Func<TEntity, float>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<float?> SumAsync(Expression<Func<TEntity, float?>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<double> SumAsync(Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<double?> SumAsync(Expression<Func<TEntity, double?>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<decimal?> SumAsync(Expression<Func<TEntity, decimal?>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        CxException NotFoundByIdException(TKey id);
        CxException EntityNotFoundException();
    }
}
