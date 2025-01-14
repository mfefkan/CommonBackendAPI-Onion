using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    public class ReadOnlyAuthDBContext : BaseDbContext
    {
        public ReadOnlyAuthDBContext(DbContextOptions<ReadOnlyAuthDBContext> options) : base(options) 
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:IgnoreMigrations", true);
            base.OnModelCreating(modelBuilder);
        }
    }
}
