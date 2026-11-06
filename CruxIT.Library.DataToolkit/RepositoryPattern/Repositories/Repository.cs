using AutoMapper.QueryableExtensions;
using CruxIT.Library.DataToolkit.EntityFramework;
using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;
using CruxIT.Library.DataToolkit.EntityFramework.Enums;
using CruxIT.Library.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.RepositoryPattern.Repositories
{
    public class Repository<TUnitOfWork, TKey, TEntity> : QueryableRepository<TUnitOfWork, TKey, TEntity>,
        IRepository<TUnitOfWork, TKey, TEntity>
        where TUnitOfWork : IUnitOfWork
        where TEntity : class, IEntityBase<TKey>
        where TKey : IEquatable<TKey>
    {
        protected virtual bool UseTrace { get; set; } = true;
        protected DbSet<TEntity> DbSet { get; set; }

        public Repository(TUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            DbSet = UnitOfWork.Set<TEntity>();
        }

        protected virtual void TrackEntityAction(TEntity entity, ActionTypes actionType)
        {

            if (!(entity is IEntity<TKey>)) return;

            IEntity<TKey> e = (IEntity<TKey>)entity;
            switch (actionType)
            {
                case ActionTypes.Add:
                    e.CreatedUserId = UnitOfWork.CurrentUserId;
                    e.CreatedUserName = UnitOfWork.CurrentUserName;
                    e.CreatedDate = DateTime.UtcNow;
                    return;
                case ActionTypes.Update:
                    e.LastUpdatedUserId = UnitOfWork.CurrentUserId;
                    e.LastUpdatedUserName = UnitOfWork.CurrentUserName;
                    e.LastUpdatedDate = DateTime.UtcNow;
                    return;
            }

            if (entity is ISoftDeleteEntity<TKey>)
            {
                ISoftDeleteEntity<TKey> d = (ISoftDeleteEntity<TKey>)e;
                d.DeletedUserId = UnitOfWork.CurrentUserId;
                d.DeletedUserName = UnitOfWork.CurrentUserName;
                d.DeletedDate = DateTime.UtcNow;
                d.IsDeleted = true;
            }
        }

        protected virtual void AddTrace(TEntity entity, ActionTypes actionType, CancellationToken cancellationToken = default)
        {
            TrackEntityAction(entity, actionType);
            if (UseTrace)
                UnitOfWork.Trace.Add<TEntity, TKey>(entity, actionType, cancellationToken);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
            AddTrace(entity, ActionTypes.Add, cancellationToken);
            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
                await AddAsync(entity, cancellationToken);
            return entities;
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return UpdateAsync(entity, ActionTypes.Update, cancellationToken);
        }

        private async Task<TEntity> UpdateAsync(TEntity entity, ActionTypes actionType = ActionTypes.Update, CancellationToken cancellationToken = default)
        {
            TEntity dbEntity;
            if (UnitOfWork.DbContext.Entry(entity).State == EntityState.Detached)
            {
                dbEntity = await GetByIdAsync(entity.Id, cancellationToken);
                UnitOfWork.DbContext.Entry(dbEntity).CurrentValues.SetValues(entity);
            }
            else dbEntity = entity;
            DbSet.Update(dbEntity);
            AddTrace(dbEntity, actionType, cancellationToken);
            return dbEntity;
        }

        public virtual async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
                await UpdateAsync(entity, cancellationToken);
            return entities;
        }

        public virtual async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is ISoftDeleteEntity<TKey>)
            {
                await UpdateAsync(entity, ActionTypes.Delete, cancellationToken);
                return;
            }
            DbSet.Remove(entity);
            AddTrace(entity, ActionTypes.Delete);
        }

        public virtual async Task RemoveRangeAsync(CancellationToken cancellationToken = default, params TEntity[] entities)
        {
            foreach (var entity in entities)
                await RemoveAsync(entity, cancellationToken);
        }
    }
}