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
            builder.Property(c => c.Correo).HasMaxLength(50);
            builder.Property(c => c.Identificador).HasMaxLength(50);


            /*builder.HasIndex(c => c.Correo).IsUnique();
            builder.HasIndex(c => c.Identificador).IsUnique();*/
        }
    }
}
