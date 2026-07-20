using Dominio.Models;
using Dominio.Repositories;
using Dominio.Repositories.Extenciones;
using Dominio.Repositories.Extensiones;
using Infraestructura.Data;
using Infraestructura.Repositories.Extenciones;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;

namespace Infraestructura.Repositories
{
    public class UsuarioExternoRepository : GenericRepository<UsuarioExterno>, IUsuarioExternoRepository
    {
        private readonly AutenticationContext dbContext;

        public UsuarioExternoRepository(AutenticationContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public IPagina<UsuarioExterno> Filter(IConsulta ownerParameters, string especificaciones)
        {
            return PagedList<UsuarioExterno>.ToPagedList(dbContext.Set<UsuarioExterno>()
                  .Where(especificaciones),
                      ownerParameters.PageNumber,
                      ownerParameters.PageSize);
        }
        public UsuarioExterno GetByIdConDependencias(int id)
        {
            return dbContext.Set<UsuarioExterno>().AsNoTracking().
                Include(c=>c.Departamento).Include(c=>c.Municipio).Include(c=>c.Nacionalidad).
                FirstOrDefault("Id="+id);
        }
    }
}
