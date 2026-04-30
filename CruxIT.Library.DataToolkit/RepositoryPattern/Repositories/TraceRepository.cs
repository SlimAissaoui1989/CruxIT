using CruxIT.Library.DataToolkit.EntityFramework;
using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;
using CruxIT.Library.DataToolkit.EntityFramework.Entities;
using CruxIT.Library.DataToolkit.EntityFramework.Enums;
using CruxIT.Library.Jsons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.RepositoryPattern.Repositories
{
    public class TraceRepository : Repository<IUnitOfWork, int, Trace>, ITraceRepository
    {
        private int _numberOfTrace = 0;
        private int _numberOfExecutingTrace = 0;
        private bool _traceIsSaved = false;
        public TraceRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            UseTrace = false;
        }

        public override sealed Task<Trace> UpdateAsync(Trace entity)
        {
            throw new NotImplementedException();
        }

        public override sealed Task RemoveAsync(Trace entity)
        {
            throw new NotImplementedException();
        }

        public override sealed Task<Trace> AddAsync(Trace entity)
        {
            throw new NotImplementedException();
        }

        public virtual void Add<TEntity, TKey>(TEntity entity, ActionTypes actionType)
            where TEntity : IEntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            _numberOfTrace++;

            UnitOfWork.SavedChangesAsync += async (s, e) =>
            {
                if (!_traceIsSaved)
                {
                    var options = new JsonSerializerOptions
                    {
                        Converters = { new SimpleTypeConverter<TEntity>() },
                        ReferenceHandler = ReferenceHandler.IgnoreCycles,
                        PropertyNamingPolicy = null,
                        WriteIndented = true
                    };

                    if (UnitOfWork.CurrentUserId != null)
                    {
                        Trace trace = new()
                        {
                            ActionType = actionType,
                            TableId = entity.Id.ToString()!,
                            TableName = entity.GetType().Name,
                            UserId = UnitOfWork.CurrentUserId ?? 0,
                            UserName = UnitOfWork.CurrentUserName!,
                            ActionDate = DateTime.UtcNow,
                            Browser = UnitOfWork.Browser,
                            Device = UnitOfWork.Device,
                            IpAdress = UnitOfWork.IpAdress,
                            OS = UnitOfWork.OS,
                            Value = JsonSerializer.Serialize(entity, options)
                        };
                        await base.AddAsync(trace);
                    }
                    _numberOfExecutingTrace++;
                    if (_numberOfTrace == _numberOfExecutingTrace)
                    {
                        _traceIsSaved = true;
                        await UnitOfWork.SaveChangesAsync();
                    }
                }
            };
        }

        public virtual async Task<List<Trace>> GetEntityTraceAsync(string tableName, string tableId)
        {
            return await this.OrderBy(o => o.ActionDate).GetAllAsync(c => c.TableName == tableName && c.TableId == tableId);
        }
    }
}
