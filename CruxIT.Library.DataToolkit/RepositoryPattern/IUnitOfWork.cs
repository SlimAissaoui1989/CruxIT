using AutoMapper;
using CruxIT.Library.DataToolkit.EntityFramework;
using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;
using CruxIT.Library.DataToolkit.RepositoryPattern.Repositories;
using CruxIT.Library.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.RepositoryPattern
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        CxDbContext DbContext { get; }
        IDbContextTransaction? CurrentTransaction { get; }
        string? CurrentLanguageId { get; set; }
        int? CurrentUserId { get; set; }
        string? CurrentUserName { get; set; }
        int? CurrentRoleId { get; set; }
        string? CurrentRoleName { get; set; }
        string? IpAdress { get; set; }
        string? Browser { get; set; }
        string? OS { get; set; }
        string? Device { get; set; }
        IMapper? Mapper { get; set; }
        ITraceRepository Trace { get; }

        public event CxAsyncEventHandler SavedChangesAsync;

        DbSet<TEntity> Set<TEntity>() where TEntity : class, IEntityBase;
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<int> SaveChangesAsync();
    }
    public interface IUnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : CxDbContext, new()
    {
        new TDbContext DbContext { get; }
        //new ITraceRepository<TDbContext> Trace { get; }
    }
}
