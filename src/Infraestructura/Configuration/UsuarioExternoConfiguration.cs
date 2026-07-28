using Dominio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infraestructura.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Configuration
{
    public class UsuarioExternoConfiguration : IEntityTypeConfiguration<UsuarioExterno>
    {
        public void Configure(EntityTypeBuilder<UsuarioExterno> builder)
        {
            builder.ToTable("usuario_externo");
            builder.Property(c => c.Email).HasMaxLength(50);
            builder.Property(c => c.Identifier).HasMaxLength(50);


            /*builder.HasIndex(c => c.Email).IsUnique();
            builder.HasIndex(c => c.Identifier).IsUnique();*/
        }
    }
}
