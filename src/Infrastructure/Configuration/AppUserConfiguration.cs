using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Configuration
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            // STG SQL (qa.smoke Id=5): columns are AccessIdentifier and Password.
            // There is no PasswordHash column — map the real names so EF cannot drop the hash.
            builder.Property(c => c.AccessIdentifier)
                .HasColumnName("AccessIdentifier")
                .HasMaxLength(100);
            builder.Property(c => c.Password)
                .HasColumnName("Password");

            builder.HasIndex(c => c.AccessIdentifier).IsUnique();
        }
    }
    
}
