using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Cotorra.General.Schema;
using Action = Cotorra.General.Schema.Action;

namespace Cotorra.General.Core.Context
{

    public static class ModelBuilderExtensions
    {
        public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static void SetIDPrimaryKey(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            { 
                var e = entity as EntityType;
                var myList = new List<string>()
                {
                   "ID"
                };
                IReadOnlyList<string> list = myList;
                e.Builder.HasKey(list, ConfigurationSource.Convention);
            }
        }
    }

    public class CotorraGeneralContext : DbContext
    {
        #region "Attributes"
        private readonly string _connectionString;
        #endregion

        #region "Constructors"      

        static CotorraGeneralContext()
        {
        }

        public CotorraGeneralContext(DbContextOptions options, string connectionString) : base(options)
        {
            _connectionString = connectionString;
        }

        public CotorraGeneralContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
            optionsBuilder.ConfigureWarnings((i =>
            {
                i.Ignore(RelationalEventId.QueryClientEvaluationWarning);
            }));            
            
            if (!string.IsNullOrEmpty(_connectionString))
            {
                var conn = new SqlConnectionStringBuilder(_connectionString)
                {
                    MaxPoolSize = 600,
                    MinPoolSize = 5
                };

                optionsBuilder.UseSqlServer(conn.ToString());
            }

            base.OnConfiguring(optionsBuilder);
        }

        public CotorraGeneralContext()
        {
        }
        #endregion

        #region "Public Properties"
        public virtual DbSet<Subscriber> Subscriber { get; set; }
        public virtual DbSet<Action> Action { get; set; }
        public virtual DbSet<ActionSubscription> ActionSubscription { get; set; }
       
        
       
        
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.SetIDPrimaryKey();
            modelBuilder.RemovePluralizingTableNameConvention();
        }

        #region "Public Methods"
        public string PluginID
        {
            get { return "16810D7A-4E31-4E7E-AFED-82192B980C6B"; }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion
    }
}
