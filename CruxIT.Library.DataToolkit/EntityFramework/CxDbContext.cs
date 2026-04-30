using CruxIT.Library.DataToolkit.DataConversions;
using CruxIT.Library.DataToolkit.EntityFramework.Entities;
using CruxIT.Library.Loggings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.EntityFramework
{
    public class CxDbContext : DbContext
    {
        public CxDbContext() : base() { }
        public CxDbContext(DbContextOptions options) : base(options) { }
        public CxDbContext(string connectionString) : this()
        {
            Database.SetConnectionString(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CxDbContext).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Create your logger instance
            var logger = new CxLogger(this.GetType().Name);

            optionsBuilder
                .EnableSensitiveDataLogging()
                .LogTo(message =>
                {
                    // Forward EF logs to CxLogger
                    logger.LogError(message);  // or LogError depending on LogLevel
                }, LogLevel.Error);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateOnly>().HaveConversion<DateOnlyConverter>().HaveColumnType("Date");
            configurationBuilder.Properties<TimeOnly>().HaveConversion<TimeOnlyConverter>().HaveColumnType("Time"); ;
            base.ConfigureConventions(configurationBuilder);
        }

        public virtual DbSet<Trace> Traces { get; set; }
    }
}
