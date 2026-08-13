using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration
{
    public static class EnglishTableConfiguration
    {
        public static void Apply(ModelBuilder builder)
        {
            builder.Entity<Catalog>().ToTable("catalog");
            builder.Entity<Permission>().ToTable("permission");
            builder.Entity<Role>().ToTable("role");
            builder.Entity<AppUser>().ToTable("app_user");
            builder.Entity<ExternalUser>().ToTable("external_user");
            builder.Entity<RolePermission>().ToTable("role_permission");
            builder.Entity<UserRole>().ToTable("user_role");
            builder.Entity<Tenant>().ToTable("tenant");
            builder.Entity<TenantModule>().ToTable("tenant_module");
        }
    }
}
