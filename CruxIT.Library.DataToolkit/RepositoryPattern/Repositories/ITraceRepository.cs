using CruxIT.Library.DataToolkit.EntityFramework;
using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;
using CruxIT.Library.DataToolkit.EntityFramework.Entities;
using CruxIT.Library.DataToolkit.EntityFramework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.RepositoryPattern.Repositories
{
    public interface ITraceRepository : IRepository<IUnitOfWork, int, Trace>
    {
        void Add<TEntity, TKey>(TEntity entity, ActionTypes actionType)
            where TEntity : IEntityBase<TKey>
            where TKey : IEquatable<TKey>;
    }

    //public interface ITraceRepository<TDbContext> : ITraceRepository
    //    where TDbContext : CxDbContext, new()
    //{
    //    void Add<TEntity, TKey>(TEntity entity, ActionTypes actionType)
    //        where TEntity : IEntityBase<TKey>
    //        where TKey : IEquatable<TKey>;
    //}
}
