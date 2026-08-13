using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class TenantModuleConfiguration : IEntityTypeConfiguration<TenantModule>
    {
        public void Configure(EntityTypeBuilder<TenantModule> builder)
        {
            builder.ToTable("tenant_module");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ModuleCode).HasMaxLength(40).IsRequired();
            builder.HasIndex(x => new { x.TenantId, x.ModuleCode }).IsUnique();
            builder.HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
