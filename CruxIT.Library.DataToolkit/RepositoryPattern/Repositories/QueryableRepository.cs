using AutoMapper.QueryableExtensions;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CruxIT.Library.DataToolkit.RepositoryPattern.Repositories
{
    public class QueryableRepository<TUnitOfWork, TKey, TEntity> : IQueryableRepository<TUnitOfWork, TKey, TEntity>
        where TUnitOfWork : IUnitOfWork
        where TEntity : class, IEntityBase<TKey>
        where TKey : IEquatable<TKey>
    {
        public TUnitOfWork UnitOfWork { get; }

        public IQueryable<TEntity> Entities { get; }

        public QueryableRepository(TUnitOfWork unitOfWork) :
            this(unitOfWork, unitOfWork.Set<TEntity>())
        {
        }

        internal QueryableRepository(TUnitOfWork unitOfWork, IQueryable<TEntity> entities)
        {
            UnitOfWork = unitOfWork;
            Entities = entities;
        }

        private void CheckAutoMapper()
        {
            if (UnitOfWork.Mapper == null)
                throw new InvalidOperationException("AutoMapper is not configured (UnitOfWork.Mapper is null).");
        }

        public virtual async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            return predicate is null
                ? await Entities.FirstOrDefaultAsync(cancellationToken)
                : await Entities.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual async Task<TResult?> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            CheckAutoMapper();

            IQueryable<TEntity> query = Entities.AsNoTracking();

            if (predicate is not null)
                query = query.Where(predicate);

            return await query
                .ProjectTo<TResult>(UnitOfWork.Mapper!.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            TEntity? entity = await GetFirstOrDefaultAsync(predicate, cancellationToken) ?? throw EntityNotFoundException();
            return entity;
        }

        public virtual async Task<TResult> GetFirstAsync<TResult>(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default)
        {
            TResult? result = await GetFirstOrDefaultAsync<TResult>(predicate, cancellationToken) ?? throw EntityNotFoundException();
            return result;
        }

        public virtual async Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await GetFirstAsync(c => c.Id.Equals(id), cancellationToken);
        }

        public virtual async Task<TResult> GetByIdAsync<TResult>(TKey id, CancellationToken cancellationToken = default)
        {
            return await GetFirstAsync<TResult>(c => c.Id.Equals(id), cancellationToken);
        }

        public virtual async Task<TEntity?> GetByIdOrDefaultAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await GetFirstOrDefaultAsync(c => c.Id.Equals(id), cancellationToken);
        }

        public virtual async Task<TResult?> GetByIdOrDefaultAsync<TResult>(TKey id, CancellationToken cancellationToken = default)
        {
            return await GetFirstOrDefaultAsync<TResult>(c => c.Id.Equals(id), cancellationToken);
        }

        public async virtual Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = Entities;

            if (predicate is not null)
                query = query.Where(predicate);

            return await query.ToListAsync(cancellationToken);
        }

        public async virtual Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            CheckAutoMapper();

            IQueryable<TEntity> query = Entities.AsNoTracking();

            if (predicate is not null)
                query = query.Where(predicate);

            return await query
                .ProjectTo<TResult>(UnitOfWork.Mapper!.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        private IQueryable<TEntity> ApplyPredicate(Expression<Func<TEntity, bool>>? predicate)
        {
            return predicate is null ? Entities : Entities.Where(predicate);
        }

        public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            return predicate is null
                ? Entities.AnyAsync(cancellationToken)
                : Entities.AnyAsync(predicate, cancellationToken);
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            return predicate is null
                ? Entities.CountAsync(cancellationToken)
                : Entities.CountAsync(predicate, cancellationToken);
        }

        public virtual async Task<(List<TEntity> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");

            var query = ApplyPredicate(predicate);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip(checked((pageNumber - 1) * pageSize))
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public virtual Task<int> SumAsync(Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
            => ApplyPredicate(predicate).SumAsync(selector, cancellationToken);

        public virtual Task<int?> SumAsync(Expression<Func<TEntity, int?>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
            => ApplyPredicate(predicate).SumAsync(selector, cancellationToken);

        public virtual Task<long> SumAsync(Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
            => ApplyPredicate(predicate).SumAsync(selector, cancellationToken);
        public virtual Task<long?> SumAsync(Expression<Func<TEntity, long?>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
            => ApplyPredicate(predicate).SumAsync(selector, cancellationToken);

        public virtual Task<float> SumAsync(Expression<Func<TEntity, float>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
            => ApplyPredicate(predicate).SumAsync(selector, cancellationToken);

        public virtual Task<float?> SumAsync(Expression<Func<TEntity, float?>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
            => ApplyPredicate(predicate).SumAsync(selector, cancellationToken);

        public virtual Task<double> SumAsync(Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
            => ApplyPredicate(predicate).SumAsync(selector, cancellationToken);

        public virtual Task<double?> SumAsync(Expression<Func<TEntity, double?>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
            => ApplyPredicate(predicate).SumAsync(selector, cancellationToken);

        public virtual Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
            => ApplyPredicate(predicate).SumAsync(selector, cancellationToken);

        public virtual Task<decimal?> SumAsync(Expression<Func<TEntity, decimal?>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
            => ApplyPredicate(predicate).SumAsync(selector, cancellationToken);

        public virtual Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            return ApplyPredicate(predicate).MinAsync(selector, cancellationToken);
        }

        public virtual Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            return ApplyPredicate(predicate).MaxAsync(selector, cancellationToken);
        }

        public virtual CxException EntityNotFoundException()
        {
            return new CxException($"{typeof(TEntity).Name} matching the given condition was not found.", CxHttpStatusCode.NotFound, "EntityNotFound");
        }

        public virtual CxException NotFoundByIdException(TKey id)
        {
            return new CxException($"Entity of type {typeof(TEntity).Name} with ID '{id}' was not found.", CxHttpStatusCode.NotFound, "IdNotFound", id);
        }

    }
}
