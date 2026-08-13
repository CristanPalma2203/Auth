

using Domain.Models;
using Domain.Service;
using Infrastructure.Configuration;
using Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Data
{
    public class AutenticationContext : DbContext
    {
        private readonly DbContextOptions<AutenticationContext> options;
        private readonly ITokenService tokenService;

        public AutenticationContext(DbContextOptions<AutenticationContext> options, ITokenService tokenService)
      : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
            this.options = options;
            this.tokenService = tokenService;
        }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ExternalUser> ExternalUsers { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantModule> TenantModules { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            PermissionSeeder.Seed(builder);
            AppUserSeeder.Seed(builder);
            ImporterPermissionSeeder.Seed(builder);
            CatalogPermissionsSeeder.Seed(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AutenticationContext).Assembly);
            EnglishTableConfiguration.Apply(builder);
            base.OnModelCreating(builder);

        }
        public override int SaveChanges()
        {
            var changedEntities = ChangeTracker.Entries();

            foreach (var changedEntity in changedEntities)
            {
                if (changedEntity.Entity is IEntityAuditable)
                {
                    var entity = (IEntityAuditable)changedEntity.Entity;

                    if (changedEntity.State == EntityState.Added)
                    {
                        entity.CreatedAt = DateTime.Now;
                        entity.CreatedByUserId = tokenService.GetUserId();
                    }
                    if (changedEntity.State == EntityState.Modified)
                    {

                        changedEntity.Context.Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                        changedEntity.Context.Entry(entity).Property(x => x.CreatedByUserId).IsModified = false;
                        entity.UpdatedAt = DateTime.Now;
                        entity.UpdatedByUserId = tokenService.GetUserId();
                    }
                }
            }

            return base.SaveChanges();
        }

    }
}
