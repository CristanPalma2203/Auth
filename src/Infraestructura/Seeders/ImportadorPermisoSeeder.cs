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
                PermisoPadre = Permiso.idPermisoAdministracion,
                Id = 13,
                Codigo = "usuarios-externos",
                EsMenu = true,
                Nombre = "Usuarios externos",
                Orden = 1,
                Url = "/usuarios-externos",
                Icono = "usuario-externo",
                Asignable = true,
                TieneHijos = true
            };
            var permisoInvitarUsuario = new Permiso
            {
                PermisoPadre = permiso.Id,
                Id = 14,
                Codigo = "gestionar-usuario-externo",
                EsMenu = false,
                Nombre = "Gestionar usuario externo",
                Orden = 1,
                Url = "/usuarios-externos/gestionar/:id",
                Asignable = true,
                TieneHijos = false
            };
            var listarPermiso = new Permiso
            {
                PermisoPadre = permiso.Id,
                Id = 15,
                Codigo = "listar-usuarios-externos",
                EsMenu = true,
                Nombre = "Usuarios externos",
                Url = "/usuarios-externos",
                Asignable = true,
                TieneHijos = false
            };
            var gestionarAccesos = new Permiso
            {
                PermisoPadre = permiso.Id,
                Id = 16,
                Codigo = "gestionar-accesos-usuario-externo",
                EsMenu = false,
                Nombre = "Gestión de accesos",
                Url = "/usuarios-externos/accesos",
                Asignable = true,
                TieneHijos = false
            };

            builder.Entity<Permiso>().HasData(permiso);
            builder.Entity<Permiso>().HasData(permisoInvitarUsuario);
            builder.Entity<Permiso>().HasData(listarPermiso);
            builder.Entity<Permiso>().HasData(gestionarAccesos);
        }
    }
}
