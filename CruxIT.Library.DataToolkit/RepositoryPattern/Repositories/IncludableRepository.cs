using CruxIT.Library.DataToolkit.EntityFramework;
using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq;

namespace CruxIT.Library.DataToolkit.RepositoryPattern.Repositories
{
    public class IncludableRepository<TUnitOfWork, TKey, TEntity, TProperty> : QueryableRepository<TUnitOfWork, TKey, TEntity>, IIncludableRepository<TUnitOfWork, TKey, TEntity, TProperty>
        where TUnitOfWork : IUnitOfWork
        where TEntity : class, IEntityBase<TKey>
        where TKey : IEquatable<TKey>
    {
        public new IIncludableQueryable<TEntity, TProperty> Entities
        {
            get => (IIncludableQueryable<TEntity, TProperty>)base.Entities;
        }

        internal IncludableRepository(TUnitOfWork unitOfWork, IIncludableQueryable<TEntity, TProperty> entities)
            : base(unitOfWork, entities)
        {
        }
    }
}
