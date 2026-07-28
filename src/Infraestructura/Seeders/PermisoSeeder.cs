using Dominio.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Seeders
{
    public static class PermisoSeeder
    {

        private static int Id = 0;


        public static void Seed(ModelBuilder builder)
        {
            var permisoModuloAutenticacion = new Permiso { Id = getId(), Code = "administracion", IsMenu = true, Name = "ADMINISTRACIÓN", SortOrder = 1, Url = "", IsAssignable=true, HasChildren = true };
            var usuarios = new Permiso { ParentPermissionId= permisoModuloAutenticacion.Id ,Id = getId(), Code = "usuarios", IsMenu = true, Name = "Usuario", SortOrder = 1, Url = "/usuario", Icon = "icon-people", IsAssignable=true, HasChildren = true };
           
            var permisoUsuario = new Permiso { ParentPermissionId= usuarios.Id, Id = getId(), Code = "usuario-listar", IsMenu = false, Name = "Lista Usuarios", SortOrder = 1, Url = "/usuario", IsAssignable = true, HasChildren = false };
            var permisoUsuarioCrear = new Permiso { ParentPermissionId = usuarios.Id, Id = getId(), Code = "usuario-crear", IsMenu = false, Name = "Crear usuario", SortOrder = 1, Url = "/usuario/crear", IsAssignable = true, HasChildren = false };
            var usuarioeditar = new Permiso { ParentPermissionId = usuarios.Id, Id = getId(), Code = "usuario-editar", IsMenu = false, Name = "Editar usuario", SortOrder = 1, Url = "/usuario/editar/:id", IsAssignable = true, HasChildren = false };
            var usuarioVer = new Permiso { ParentPermissionId = usuarios.Id, Id = getId(), Code = "usuario-ver", IsMenu = false, Name = "Ver usuario", SortOrder = 1, Url = "/usuario/ver/:id", IsAssignable = true, HasChildren = false };

            var permisoPerfilUsuario = new Permiso { Id = getId(), ParentPermissionId = usuarios.Id, Code = "perfil-usuario", IsMenu = false, Name = "Perfil de usuario", SortOrder = 1, Url = "/usuario/perfil/:id", IsAssignable = true, HasChildren = false };

            var roles = new Permiso { ParentPermissionId = permisoModuloAutenticacion.Id , Id = getId(), Code = "roles", IsMenu = true, Name = "Rol", SortOrder = 1, Url = "/rol", Icon = "icon-key", IsAssignable = true, HasChildren = true };
           
            var permisoRol = new Permiso { ParentPermissionId = roles.Id, Id = getId(), Code = "rol-listar", IsMenu = false, Name = "Lista roles", SortOrder = permisoUsuario.SortOrder + 1, Url = "/rol", IsAssignable = true, HasChildren = false };
            var permisoRolCrear = new Permiso { ParentPermissionId = roles.Id, Id = getId(), Code = "rol-crear", IsMenu = false, Name = "Crear rol", SortOrder = 1, Url = "/rol/crear", IsAssignable = true, HasChildren = false };
            var editarRol = new Permiso { ParentPermissionId = roles.Id, Id = getId(), Code = "rol-editar", IsMenu = false, Name = "Editar rol", SortOrder = 1, Url = "/rol/editar/:id", IsAssignable = true, HasChildren = false };
            var verRol = new Permiso { ParentPermissionId = roles.Id, Id = getId(), Code = "rol-ver", IsMenu = false, Name = "Ver rol", SortOrder = 1, Url = "/rol/ver/:id", IsAssignable = true, HasChildren = false };


            
            var rolAdminitracionSistema = new Rol { Id = Rol.IdRolAdministracionSistema, IsAssignable = false, Description = "Rol para la administracion del sistema", CreatedAt = new DateTime(2020, 4, 27), Name = "Administración del Sistema" };
            var rolPermisoAdmin = new RolPermiso { Id = getId2(), PermisoId = permisoModuloAutenticacion.Id, RolId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin2 = new RolPermiso { Id = getId2(), PermisoId = permisoUsuario.Id, RolId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin3 = new RolPermiso { Id = getId2(), PermisoId = permisoUsuarioCrear.Id, RolId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin4 = new RolPermiso { Id = getId2(), PermisoId = usuarioeditar.Id, RolId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin5 = new RolPermiso { Id = getId2(), PermisoId = permisoPerfilUsuario.Id, RolId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin6 = new RolPermiso { Id = getId2(), PermisoId = permisoRol.Id, RolId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin7 = new RolPermiso { Id = getId2(), PermisoId = permisoRolCrear.Id, RolId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin8 = new RolPermiso { Id = getId2(), PermisoId = editarRol.Id, RolId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin9 = new RolPermiso { Id = getId2(), PermisoId = verRol.Id, RolId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin10 = new RolPermiso { Id = getId2(), PermisoId = usuarios.Id, RolId = rolAdminitracionSistema.Id };



            builder.Entity<Rol>().HasData(rolAdminitracionSistema);
            builder.Entity<RolPermiso>().HasData(rolPermisoAdmin);
            builder.Entity<RolPermiso>().HasData(rolPermisoAdmin2);
            builder.Entity<RolPermiso>().HasData(rolPermisoAdmin3);
            builder.Entity<RolPermiso>().HasData(rolPermisoAdmin4);
            builder.Entity<RolPermiso>().HasData(rolPermisoAdmin5);
            builder.Entity<RolPermiso>().HasData(rolPermisoAdmin6);
            builder.Entity<RolPermiso>().HasData(rolPermisoAdmin7);
            builder.Entity<RolPermiso>().HasData(rolPermisoAdmin8);
            builder.Entity<RolPermiso>().HasData(rolPermisoAdmin9);
            builder.Entity<RolPermiso>().HasData(rolPermisoAdmin10);

            builder.Entity<Permiso>().HasData(permisoModuloAutenticacion);
            builder.Entity<Permiso>().HasData(usuarios);
            builder.Entity<Permiso>().HasData(permisoUsuario);
            builder.Entity<Permiso>().HasData(permisoUsuarioCrear);
            builder.Entity<Permiso>().HasData(usuarioeditar);
            builder.Entity<Permiso>().HasData(usuarioVer);
            builder.Entity<Permiso>().HasData(permisoPerfilUsuario);

            builder.Entity<Permiso>().HasData(roles);
            builder.Entity<Permiso>().HasData(permisoRol);
            builder.Entity<Permiso>().HasData(permisoRolCrear);
            builder.Entity<Permiso>().HasData(editarRol);
            builder.Entity<Permiso>().HasData(verRol);
        }

        private static int getId()
        {
            Id = Id + 1;
            return Id;
        }
        private static int getId2()
        {
            Id = Id + 1;
            return Id;
        }
    }
}
