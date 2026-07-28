using Dominio.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Seeders
{
    public static class PermisoCatalogosSeeder
    {



        public static void Seed(ModelBuilder builder)
        {
            var catalogoRoot = new Permiso { ParentPermissionId= Permiso.idPermisoAdministracion, Id = 17, Code = "catalogos", IsMenu = true, Name = "Catalogos", SortOrder = 1, Url = "/catalogos", Icon = "icon-book-open", IsAssignable=true, HasChildren = true };
            var catalogoLista = new Permiso { ParentPermissionId = catalogoRoot.Id, Id = 21, Code = "catalogo-ver", IsMenu = false, Name = "Ver catalogos", SortOrder = 1, Url = "/catalogo/ver/:id", IsAssignable = true, HasChildren = false };
            var catalogoVer= new Permiso { ParentPermissionId= catalogoRoot.Id, Id =18, Code = "catalogo-listar", IsMenu = false, Name = "Lista catalogos", SortOrder = 1, Url = "/catalogo", IsAssignable = true, HasChildren = false };
            var catalogocrear = new Permiso { ParentPermissionId = catalogoRoot.Id, Id = 19, Code = "catalogo-crear", IsMenu = false, Name = "Crear catalogo", SortOrder = 1, Url = "/catalogo/crear", IsAssignable = true, HasChildren = false };
            var catalogoEditar = new Permiso { ParentPermissionId = catalogoRoot.Id, Id = 20, Code = "catalogo-editar", IsMenu = false, Name = "Editar catalogo", SortOrder = 1, Url = "/catalogo/editar/:id", IsAssignable = true, HasChildren = false };
            builder.Entity<Permiso>().HasData(catalogoVer);
            builder.Entity<Permiso>().HasData(catalogoRoot);
            builder.Entity<Permiso>().HasData(catalogoLista);
            builder.Entity<Permiso>().HasData(catalogocrear);
            builder.Entity<Permiso>().HasData(catalogoEditar);

        }

    }
}
