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


            /*builder.HasIndex(c => c.Email).IsUnique();
            builder.HasIndex(c => c.Identifier).IsUnique();*/
        }
    }
}
