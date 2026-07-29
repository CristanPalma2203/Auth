using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Seeders
{
    public static class CatalogPermissionsSeeder
    {



        public static void Seed(ModelBuilder builder)
        {
            var catalogoRoot = new Permission { ParentPermissionId= Permission.idPermisoAdministracion, Id = 17, Code = "catalogs", IsMenu = true, Name = "Catalogos", SortOrder = 1, Url = "/catalogs", Icon = "icon-book-open", IsAssignable=true, HasChildren = true };
            var catalogoLista = new Permission { ParentPermissionId = catalogoRoot.Id, Id = 21, Code = "catalog-view", IsMenu = false, Name = "View catalog", SortOrder = 1, Url = "/catalogs/view/:id", IsAssignable = true, HasChildren = false };
            var catalogoVer= new Permission { ParentPermissionId= catalogoRoot.Id, Id =18, Code = "catalog-list", IsMenu = false, Name = "Catalog list", SortOrder = 1, Url = "/catalogs", IsAssignable = true, HasChildren = false };
            var catalogocrear = new Permission { ParentPermissionId = catalogoRoot.Id, Id = 19, Code = "catalog-create", IsMenu = false, Name = "Create catalog", SortOrder = 1, Url = "/catalogs/create", IsAssignable = true, HasChildren = false };
            var catalogoEditar = new Permission { ParentPermissionId = catalogoRoot.Id, Id = 20, Code = "catalog-edit", IsMenu = false, Name = "Edit catalog", SortOrder = 1, Url = "/catalogs/edit/:id", IsAssignable = true, HasChildren = false };
            builder.Entity<Permission>().HasData(catalogoVer);
            builder.Entity<Permission>().HasData(catalogoRoot);
            builder.Entity<Permission>().HasData(catalogoLista);
            builder.Entity<Permission>().HasData(catalogocrear);
            builder.Entity<Permission>().HasData(catalogoEditar);

        }

    }
}
