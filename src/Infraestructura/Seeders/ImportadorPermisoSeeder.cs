using Dominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Seeders
{
    public class ImportadorPermisoSeeder
    {
        public static void Seed(ModelBuilder builder)
        {
            var permiso = new Permiso
            {
                ParentPermissionId = Permiso.idPermisoAdministracion,
                Id = 13,
                Code = "usuarios-externos",
                IsMenu = true,
                Name = "Usuarios externos",
                SortOrder = 1,
                Url = "/usuarios-externos",
                Icon = "usuario-externo",
                IsAssignable = true,
                HasChildren = true
            };
            var permisoInvitarUsuario = new Permiso
            {
                ParentPermissionId = permiso.Id,
                Id = 14,
                Code = "gestionar-usuario-externo",
                IsMenu = false,
                Name = "Gestionar usuario externo",
                SortOrder = 1,
                Url = "/usuarios-externos/gestionar/:id",
                IsAssignable = true,
                HasChildren = false
            };
            var listarPermiso = new Permiso
            {
                ParentPermissionId = permiso.Id,
                Id = 15,
                Code = "listar-usuarios-externos",
                IsMenu = true,
                Name = "Usuarios externos",
                Url = "/usuarios-externos",
                IsAssignable = true,
                HasChildren = false
            };
            var gestionarAccesos = new Permiso
            {
                ParentPermissionId = permiso.Id,
                Id = 16,
                Code = "gestionar-accesos-usuario-externo",
                IsMenu = false,
                Name = "Gestión de accesos",
                Url = "/usuarios-externos/accesos",
                IsAssignable = true,
                HasChildren = false
            };

            builder.Entity<Permiso>().HasData(permiso);
            builder.Entity<Permiso>().HasData(permisoInvitarUsuario);
            builder.Entity<Permiso>().HasData(listarPermiso);
            builder.Entity<Permiso>().HasData(gestionarAccesos);
        }
    }
}
