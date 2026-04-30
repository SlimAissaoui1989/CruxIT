using AutoMapper;
using CruxIT.Library.DataToolkit.EntityFramework;
using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;
using CruxIT.Library.DataToolkit.RepositoryPattern.Repositories;
using CruxIT.Library.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.RepositoryPattern
{
    public class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext>
        where TDbContext : CxDbContext, new()// no need for `new()` when injecting
    {
        private readonly TDbContext _context;
        private bool _disposed;
        private bool _raisingSavedChanges; // re-entrancy guard

        public event CxAsyncEventHandler? SavedChangesAsync;

        public TDbContext DbContext => _context;
        CxDbContext IUnitOfWork.DbContext => DbContext;

        public string? CurrentLanguageId { get; set; }
        public int? CurrentUserId { get; set; }
        public string? CurrentUserName { get; set; }
        public int? CurrentRoleId { get; set; }
        public string? CurrentRoleName { get; set; }
        public virtual string? IpAdress { get; set; }
        public virtual string? Browser { get; set; }
        public virtual string? OS { get; set; }
        public virtual string? Device { get; set; }

        public IMapper? Mapper { get; set; }

        public IDbContextTransaction? CurrentTransaction => _context.Database.CurrentTransaction;

        public ITraceRepository Trace => GetRepository<TraceRepository>();

        private Dictionary<Type, object> Repositories { get; set; } = new Dictionary<Type, object>();

        public UnitOfWork(TDbContext context, IMapper? mapper = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Mapper = mapper;
            Repositories = new Dictionary<Type, object>();
        }

        protected TRepository GetRepository<TRepository>()
            where TRepository : class
        {
            Type type = typeof(TRepository);
            if (!Repositories.ContainsKey(type))
            {
                Repositories[type] = Activator.CreateInstance(type, this)!;
            }
            return (TRepository)Repositories[type];
        }

        private async Task OnSavedChangesAsync()
        {
            // Prevent re-entrancy (handlers calling SaveChangesAsync again)
            if (_raisingSavedChanges) return;

            var handlers = SavedChangesAsync;
            if (handlers is null)
                return;

            _raisingSavedChanges = true;
            try
            {
                // Multicast async events: invoke all and await all
                var invocationList = handlers.GetInvocationList();
                var tasks = new List<Task>(invocationList.Length);
                foreach (var d in invocationList)
                {
                    if (d is CxAsyncEventHandler h)
                        tasks.Add(SafeInvokeAsync(h, this));
                }
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            finally
            {
                _raisingSavedChanges = false;
            }

            static async Task SafeInvokeAsync(CxAsyncEventHandler handler, object sender)
            {
                // Ensure exceptions are surfaced to WhenAll (no async-void)
                await handler.Invoke(sender, EventArgs.Empty).ConfigureAwait(false);
            }
        }

        public Task<int> SaveChangesAsync()
            => SaveChangesAsync(CancellationToken.None);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            EnsureNotDisposed();

            var result = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // If event handlers throw, you probably want the exception to bubble up
            // so callers know the pipeline failed. If you prefer “log and continue”,
            // wrap OnSavedChangesAsync() in try/catch and log instead.
            await OnSavedChangesAsync().ConfigureAwait(false);

            return result;
        }

        public DbSet<TEntity> Set<TEntity>()
            where TEntity : class, IEntityBase
        {
            return DbContext.Set<TEntity>();
        }

        public Task<IDbContextTransaction> BeginTransactionAsync()
            => _context.Database.BeginTransactionAsync(CancellationToken.None);

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
            => _context.Database.BeginTransactionAsync(cancellationToken);

        public Task<IDbContextTransaction> BeginTransactionAsync(System.Data.IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
            => _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);

        public void Dispose()
        {
            if (_disposed) return;
            _context.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            // Prefer async dispose when available
            await _context.DisposeAsync().ConfigureAwait(false);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        private void EnsureNotDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().Name);
        }
    }

}
