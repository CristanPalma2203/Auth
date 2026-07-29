using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Seeders
{
    public static class PermissionSeeder
    {

        private static int Id = 0;


        public static void Seed(ModelBuilder builder)
        {
            var permisoModuloAutenticacion = new Permission { Id = getId(), Code = "administration", IsMenu = true, Name = "ADMINISTRACIÓN", SortOrder = 1, Url = "", IsAssignable=true, HasChildren = true };
            var users = new Permission { ParentPermissionId= permisoModuloAutenticacion.Id ,Id = getId(), Code = "users", IsMenu = true, Name = "AppUser", SortOrder = 1, Url = "/users", Icon = "icon-people", IsAssignable=true, HasChildren = true };
           
            var permisoUsuario = new Permission { ParentPermissionId= users.Id, Id = getId(), Code = "user-list", IsMenu = false, Name = "Lista Usuarios", SortOrder = 1, Url = "/users", IsAssignable = true, HasChildren = false };
            var permisoUsuarioCrear = new Permission { ParentPermissionId = users.Id, Id = getId(), Code = "user-create", IsMenu = false, Name = "Crear usuario", SortOrder = 1, Url = "/users/crear", IsAssignable = true, HasChildren = false };
            var usuarioeditar = new Permission { ParentPermissionId = users.Id, Id = getId(), Code = "user-edit", IsMenu = false, Name = "Editar usuario", SortOrder = 1, Url = "/users/editar/:id", IsAssignable = true, HasChildren = false };
            var usuarioVer = new Permission { ParentPermissionId = users.Id, Id = getId(), Code = "user-view", IsMenu = false, Name = "Ver usuario", SortOrder = 1, Url = "/users/ver/:id", IsAssignable = true, HasChildren = false };

            var permisoPerfilUsuario = new Permission { Id = getId(), ParentPermissionId = users.Id, Code = "user-profile", IsMenu = false, Name = "Perfil de usuario", SortOrder = 1, Url = "/users/perfil/:id", IsAssignable = true, HasChildren = false };

            var roles = new Permission { ParentPermissionId = permisoModuloAutenticacion.Id , Id = getId(), Code = "roles", IsMenu = true, Name = "Role", SortOrder = 1, Url = "/roles", Icon = "icon-key", IsAssignable = true, HasChildren = true };
           
            var permisoRol = new Permission { ParentPermissionId = roles.Id, Id = getId(), Code = "role-list", IsMenu = false, Name = "Lista roles", SortOrder = permisoUsuario.SortOrder + 1, Url = "/roles", IsAssignable = true, HasChildren = false };
            var permisoRolCrear = new Permission { ParentPermissionId = roles.Id, Id = getId(), Code = "role-create", IsMenu = false, Name = "Crear Roles", SortOrder = 1, Url = "/roles/crear", IsAssignable = true, HasChildren = false };
            var editarRol = new Permission { ParentPermissionId = roles.Id, Id = getId(), Code = "role-edit", IsMenu = false, Name = "Editar Roles", SortOrder = 1, Url = "/roles/editar/:id", IsAssignable = true, HasChildren = false };
            var verRol = new Permission { ParentPermissionId = roles.Id, Id = getId(), Code = "role-view", IsMenu = false, Name = "Ver Roles", SortOrder = 1, Url = "/roles/ver/:id", IsAssignable = true, HasChildren = false };


            
            var rolAdminitracionSistema = new Role { Id = Role.IdRolAdministracionSistema, IsAssignable = false, Description = "Role para la administration del sistema", CreatedAt = new DateTime(2020, 4, 27), Name = "Administración del Sistema" };
            var rolPermisoAdmin = new RolePermission { Id = getId2(), PermissionId = permisoModuloAutenticacion.Id, RoleId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin2 = new RolePermission { Id = getId2(), PermissionId = permisoUsuario.Id, RoleId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin3 = new RolePermission { Id = getId2(), PermissionId = permisoUsuarioCrear.Id, RoleId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin4 = new RolePermission { Id = getId2(), PermissionId = usuarioeditar.Id, RoleId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin5 = new RolePermission { Id = getId2(), PermissionId = permisoPerfilUsuario.Id, RoleId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin6 = new RolePermission { Id = getId2(), PermissionId = permisoRol.Id, RoleId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin7 = new RolePermission { Id = getId2(), PermissionId = permisoRolCrear.Id, RoleId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin8 = new RolePermission { Id = getId2(), PermissionId = editarRol.Id, RoleId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin9 = new RolePermission { Id = getId2(), PermissionId = verRol.Id, RoleId = rolAdminitracionSistema.Id };
            var rolPermisoAdmin10 = new RolePermission { Id = getId2(), PermissionId = users.Id, RoleId = rolAdminitracionSistema.Id };



            builder.Entity<Role>().HasData(rolAdminitracionSistema);
            builder.Entity<RolePermission>().HasData(rolPermisoAdmin);
            builder.Entity<RolePermission>().HasData(rolPermisoAdmin2);
            builder.Entity<RolePermission>().HasData(rolPermisoAdmin3);
            builder.Entity<RolePermission>().HasData(rolPermisoAdmin4);
            builder.Entity<RolePermission>().HasData(rolPermisoAdmin5);
            builder.Entity<RolePermission>().HasData(rolPermisoAdmin6);
            builder.Entity<RolePermission>().HasData(rolPermisoAdmin7);
            builder.Entity<RolePermission>().HasData(rolPermisoAdmin8);
            builder.Entity<RolePermission>().HasData(rolPermisoAdmin9);
            builder.Entity<RolePermission>().HasData(rolPermisoAdmin10);

            builder.Entity<Permission>().HasData(permisoModuloAutenticacion);
            builder.Entity<Permission>().HasData(users);
            builder.Entity<Permission>().HasData(permisoUsuario);
            builder.Entity<Permission>().HasData(permisoUsuarioCrear);
            builder.Entity<Permission>().HasData(usuarioeditar);
            builder.Entity<Permission>().HasData(usuarioVer);
            builder.Entity<Permission>().HasData(permisoPerfilUsuario);

            builder.Entity<Permission>().HasData(roles);
            builder.Entity<Permission>().HasData(permisoRol);
            builder.Entity<Permission>().HasData(permisoRolCrear);
            builder.Entity<Permission>().HasData(editarRol);
            builder.Entity<Permission>().HasData(verRol);
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
