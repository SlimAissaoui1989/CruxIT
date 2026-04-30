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
            :base(unitOfWork)
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
                    e.CreatedUserId = UnitOfWork.CurrentUserId.GetValueOrDefault();
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

        protected virtual void AddTrace(TEntity entity, ActionTypes actionType)
        {
            TrackEntityAction(entity, actionType);
            if (UseTrace)
                UnitOfWork.Trace.Add<TEntity, TKey>(entity, actionType);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
            AddTrace(entity, ActionTypes.Add);
            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                await AddAsync(entity);
            return entities;
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            return UpdateAsync(entity, ActionTypes.Update);
        }

        private async Task<TEntity> UpdateAsync(TEntity entity, ActionTypes actionType = ActionTypes.Update)
        {
            TEntity dbEntity;
            if (UnitOfWork.DbContext.Entry(entity).State == EntityState.Detached)
            {
                dbEntity = await GetByIdAsync(entity.Id);
                UnitOfWork.DbContext.Entry(dbEntity).CurrentValues.SetValues(entity);
            }
            else dbEntity = entity;
            DbSet.Update(dbEntity);
            AddTrace(dbEntity, actionType);
            return dbEntity;
        }

        public virtual async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                await UpdateAsync(entity);
            return entities;
        }

        public virtual async Task RemoveAsync(TEntity entity)
        {
            if (entity is ISoftDeleteEntity<TKey>)
            {
                await UpdateAsync(entity, ActionTypes.Delete);
                return;
            }
            DbSet.Remove(entity);
            AddTrace(entity, ActionTypes.Delete);
        }

        public virtual async Task RemoveRangeAsync(params TEntity[] entities)
        {
            foreach (var entity in entities)
                await RemoveAsync(entity);
        }
    }
}