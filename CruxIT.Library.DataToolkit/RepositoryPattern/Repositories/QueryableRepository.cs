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

            return await Entities.ToListAsync(cancellationToken);
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

        public virtual async Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default)
        {
            return await Entities.SumAsync(selector, cancellationToken);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = Entities;

            if (predicate is not null)
                query = query.Where(predicate);

            return await Entities.CountAsync(cancellationToken);
        }

        public virtual CxException EntityNotFoundException()
        {
            return new CxException($"{typeof(TEntity).Name} matching the given condition was not found.", CxExceptionTypes.NotFound, "EntityNotFound");
        }

        public virtual CxException NotFoundByIdException(TKey id)
        {
            return new CxException($"Entity of type {typeof(TEntity).Name} with ID '{id}' was not found.", CxExceptionTypes.NotFound, "IdNotFound", id);
        }

    }
}
