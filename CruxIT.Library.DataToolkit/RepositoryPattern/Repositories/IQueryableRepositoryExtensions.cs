using CruxIT.Library.DataToolkit.EntityFramework;
using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace CruxIT.Library.DataToolkit.RepositoryPattern.Repositories
{
    public static class IQueryableRepositoryExtensions
    {
        public static IQueryableRepository<TUnitOfWork, TKey, TEntity> Where<TUnitOfWork, TKey, TEntity>(this IQueryableRepository<TUnitOfWork, TKey, TEntity> repository, Expression<Func<TEntity, bool>> predicate)
            where TUnitOfWork : IUnitOfWork
            where TEntity : class, IEntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            IQueryable<TEntity> query = repository.Entities.Where(predicate);
            return new QueryableRepository<TUnitOfWork, TKey, TEntity>(repository.UnitOfWork, query);
        }

        public static IQueryableRepository<TUnitOfWork, TKey, TEntity> AsNoTracking<TUnitOfWork, TKey, TEntity>(this IQueryableRepository<TUnitOfWork, TKey, TEntity> repository)
            where TUnitOfWork : IUnitOfWork
            where TEntity : class, IEntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            IQueryable<TEntity> query = repository.Entities.AsNoTracking();
            return new QueryableRepository<TUnitOfWork, TKey, TEntity>(repository.UnitOfWork, query);
        }

        public static IQueryableRepository<TUnitOfWork, TKey, TEntity> AsTracking<TUnitOfWork, TKey, TEntity>(this IQueryableRepository<TUnitOfWork, TKey, TEntity> repository)
            where TUnitOfWork : IUnitOfWork
            where TEntity : class, IEntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            IQueryable<TEntity> query = repository.Entities.AsTracking();
            return new QueryableRepository<TUnitOfWork, TKey, TEntity>(repository.UnitOfWork, query);
        }

        public static IQueryableRepository<TUnitOfWork, TKey, TEntity> OrderBy<TUnitOfWork, TKey, TEntity, TOrderKey>(this IQueryableRepository<TUnitOfWork, TKey, TEntity> repository, Expression<Func<TEntity, TOrderKey>> keySelector)
            where TUnitOfWork : IUnitOfWork
            where TEntity : class, IEntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            IQueryable<TEntity> query = repository.Entities.OrderBy(keySelector);
            return new QueryableRepository<TUnitOfWork, TKey, TEntity>(repository.UnitOfWork, query);
        }

        public static IQueryableRepository<TUnitOfWork, TKey, TEntity> OrderByDescending<TUnitOfWork, TKey, TEntity, TOrderKey>(this IQueryableRepository<TUnitOfWork, TKey, TEntity> repository, Expression<Func<TEntity, TOrderKey>> keySelector)
            where TUnitOfWork : IUnitOfWork
            where TEntity : class, IEntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            IQueryable<TEntity> query = repository.Entities.OrderByDescending(keySelector);
            return new QueryableRepository<TUnitOfWork, TKey, TEntity>(repository.UnitOfWork, query);
        }

        public static IQueryableRepository<TUnitOfWork, TKey, TEntity> Skip<TUnitOfWork, TKey, TEntity>(this IQueryableRepository<TUnitOfWork, TKey, TEntity> repository, int count)
            where TUnitOfWork : IUnitOfWork
            where TEntity : class, IEntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            IQueryable<TEntity> query = repository.Entities.Skip(count);
            return new QueryableRepository<TUnitOfWork, TKey, TEntity>(repository.UnitOfWork, query);
        }

        public static IQueryableRepository<TUnitOfWork, TKey, TEntity> Take<TUnitOfWork, TKey, TEntity>(this IQueryableRepository<TUnitOfWork, TKey, TEntity> repository, int count)
            where TUnitOfWork : IUnitOfWork
            where TEntity : class, IEntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            IQueryable<TEntity> query = repository.Entities.Take(count);
            return new QueryableRepository<TUnitOfWork, TKey, TEntity>(repository.UnitOfWork, query);
        }

        public static IQueryableRepository<TUnitOfWork, TKey, TEntity> Distinct<TUnitOfWork, TKey, TEntity>(this IQueryableRepository<TUnitOfWork, TKey, TEntity> repository, IEqualityComparer<TEntity>? comparer = null)
            where TUnitOfWork : IUnitOfWork
            where TEntity : class, IEntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            IQueryable<TEntity> query = repository.Entities.Distinct(comparer);
            return new QueryableRepository<TUnitOfWork, TKey, TEntity>(repository.UnitOfWork, query);
        }

        public static IQueryableRepository<TUnitOfWork, TKey, TEntity> Distinct<TUnitOfWork, TKey, TEntity, TDistinctKey>(this IQueryableRepository<TUnitOfWork, TKey, TEntity> repository, Expression<Func<TEntity, TDistinctKey>> keySelector)
            where TUnitOfWork : IUnitOfWork
            where TEntity : class, IEntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            IQueryable<TEntity> query = repository.Entities.DistinctBy(keySelector);
            return new QueryableRepository<TUnitOfWork, TKey, TEntity>(repository.UnitOfWork, query);
        }

        public static IIncludableRepository<TUnitOfWork, TKey, TEntity, TProperty> Include<TUnitOfWork, TKey, TEntity, TProperty>(this IQueryableRepository<TUnitOfWork, TKey, TEntity> repository, Expression<Func<TEntity, TProperty>> navigationPropertyPath)
            where TUnitOfWork : IUnitOfWork
            where TEntity : class, IEntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            IIncludableQueryable<TEntity, TProperty> query = repository.Entities.Include(navigationPropertyPath);

            return new IncludableRepository<TUnitOfWork, TKey, TEntity, TProperty>(repository.UnitOfWork, query);

        }

        public static IIncludableRepository<TUnitOfWork, TKey, TEntity, TProperty> ThenInclude<TUnitOfWork, TKey, TEntity, TPreviousProperty, TProperty>(this IIncludableRepository<TUnitOfWork, TKey, TEntity, TPreviousProperty> repository, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            where TUnitOfWork : IUnitOfWork
            where TEntity : class, IEntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            IIncludableQueryable<TEntity, TProperty> query = repository.Entities.ThenInclude(navigationPropertyPath);

            return new IncludableRepository<TUnitOfWork, TKey, TEntity, TProperty>(repository.UnitOfWork, query);
        }

        public static IIncludableRepository<TUnitOfWork, TKey, TEntity, TProperty> ThenInclude<TUnitOfWork, TKey, TEntity, TPreviousProperty, TProperty>
            (this IIncludableRepository<TUnitOfWork, TKey, TEntity, IEnumerable<TPreviousProperty>> repository, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            where TUnitOfWork : IUnitOfWork
            where TEntity : class, IEntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            IIncludableQueryable<TEntity, TProperty> query = repository.Entities.ThenInclude(navigationPropertyPath);

            return new IncludableRepository<TUnitOfWork, TKey, TEntity, TProperty>(repository.UnitOfWork, query);
        }
    }
}
