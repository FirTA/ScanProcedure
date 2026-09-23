using Microsoft.EntityFrameworkCore;
using ScanProcedure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanProcedure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
            public DbSet<Table> Tables => Set<Table>();
            public DbSet<Procedure> Procedures => Set<Procedure>();
            public DbSet<Menu> Menus => Set<Menu>();
            public DbSet<User> Users => Set<User>();
            public DbSet<MappingProcedure> MappingProcedures => Set<MappingProcedure>();
            public DbSet<MappingMenu> MappingMenus => Set<MappingMenu>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MappingMenu>()
                .Property(propertyExpression: p => p.TableAccess).HasConversion<string>();
            modelBuilder.Entity<MappingProcedure>()
                .Property(propertyExpression: p => p.TableAccess).HasConversion<string>();

            modelBuilder.Entity<MappingMenu>()
                .HasIndex(mm => mm.MenuId)
                .IsUnique();

            modelBuilder.Entity<MappingProcedure>()
                .HasIndex(mm => mm.ProcedureId)
                .IsUnique();
        }
    }
}
