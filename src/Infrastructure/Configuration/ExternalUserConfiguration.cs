using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Configuration
{
    public class ExternalUserConfiguration : IEntityTypeConfiguration<ExternalUser>
    {
        public void Configure(EntityTypeBuilder<ExternalUser> builder)
        {
            builder.ToTable("external_user");
            builder.Property(c => c.Email).HasMaxLength(50);
            builder.Property(c => c.Identifier).HasMaxLength(50);
            builder.Property(c => c.TenantId);

            // Nav names ≠ FK names → sin esto EF inventa StoredFileId / TipoPersonaId / UserGentionId
            builder.HasOne(c => c.StoredFile)
                .WithMany()
                .HasForeignKey(c => c.FileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.TipoPersona)
                .WithMany()
                .HasForeignKey(c => c.PersonTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.UserGention)
                .WithMany()
                .HasForeignKey(c => c.ManagedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            /*builder.HasIndex(c => c.Email).IsUnique();
            builder.HasIndex(c => c.Identifier).IsUnique();*/
        }
    }
}
