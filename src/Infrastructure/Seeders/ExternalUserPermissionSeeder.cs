using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeders
{
    public class ImporterPermissionSeeder
    {
        public static void Seed(ModelBuilder builder)
        {
            var Permissions = new Permission
            {
                ParentPermissionId = Permission.idPermisoAdministracion,
                Id = 13,
                Code = "external-users",
                IsMenu = true,
                Name = "Usuarios externos",
                SortOrder = 1,
                Url = "/external-users",
                Icon = "external-user",
                IsAssignable = true,
                HasChildren = true
            };
            var permisoInvitarUsuario = new Permission
            {
                ParentPermissionId = Permissions.Id,
                Id = 14,
                Code = "manage-external-user",
                IsMenu = false,
                Name = "Gestionar external user",
                SortOrder = 1,
                Url = "/external-users/gestionar/:id",
                IsAssignable = true,
                HasChildren = false
            };
            var listarPermiso = new Permission
            {
                ParentPermissionId = Permissions.Id,
                Id = 15,
                Code = "external-user-list",
                IsMenu = true,
                Name = "Usuarios externos",
                Url = "/external-users",
                IsAssignable = true,
                HasChildren = false
            };
            var gestionarAccesos = new Permission
            {
                ParentPermissionId = Permissions.Id,
                Id = 16,
                Code = "manage-external-user-access",
                IsMenu = false,
                Name = "Gestión de accesos",
                Url = "/external-users/accesos",
                IsAssignable = true,
                HasChildren = false
            };

            builder.Entity<Permission>().HasData(Permissions);
            builder.Entity<Permission>().HasData(permisoInvitarUsuario);
            builder.Entity<Permission>().HasData(listarPermiso);
            builder.Entity<Permission>().HasData(gestionarAccesos);
        }
    }
}
