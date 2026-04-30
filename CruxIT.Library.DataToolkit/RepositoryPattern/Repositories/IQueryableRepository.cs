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

    //public interface IQueryableRepository<TEntity>
    //    where TEntity : class, IEntityBase
    //{
    //    IQueryable<TEntity> Entities { get; }
    //    IUnitOfWork UnitOfWork { get; }
    //}

    //public interface IQueryableRepository<TUnitOfWork, TEntity>
    //    where TUnitOfWork : IUnitOfWork
    //    where TEntity : class, IEntityBase
    //{
    //    TUnitOfWork UnitOfWork { get; }
    //    Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate);
    //    Task<TResult?> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null);
    //    Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>>? predicate);
    //    Task<TResult> GetFirstAsync<TResult>(Expression<Func<TEntity, bool>>? predicate);
    //    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null);
    //    Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null);
    //    CxException EntityNotFoundException();

    //}

    public interface IQueryableRepository<TUnitOfWork, TKey, TEntity> //: IQueryableRepository<TUnitOfWork, TEntity>
        where TUnitOfWork : IUnitOfWork
        where TEntity : class, IEntityBase<TKey>
        where TKey : IEquatable<TKey>
    {
        IQueryable<TEntity> Entities { get; }
        TUnitOfWork UnitOfWork { get; }
        Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate);
        Task<TResult?> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null);
        Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>>? predicate);
        Task<TResult> GetFirstAsync<TResult>(Expression<Func<TEntity, bool>>? predicate);
        Task<TEntity> GetByIdAsync(TKey id);
        Task<TResult> GetByIdAsync<TResult>(TKey id);
        Task<TEntity?> GetByIdOrDefaultAsync(TKey id);
        Task<TResult?> GetByIdOrDefaultAsync<TResult>(TKey id);
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null);
        Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null);
        CxException NotFoundByIdException(TKey id);
        CxException EntityNotFoundException();
    }
}
