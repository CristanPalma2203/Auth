using Application.CommandHandlers;
using Application.Commands;
using Application.Dtos;
using Application.Services.PermissionQuery;
using Domain.Models;
using Domain.Repositories;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Application.Test.ComandHanlderTest
{
    public class ListPermissionsHandlerTest
    {
        private static int Id = 0;

        [TestCase]
        public void consultaPermisos_retornaLitaEstructurada() {
            var mockRepo =new Mock<IPermissionRepository>();
            mockRepo.Setup(p => p.Filter(It.IsAny<Domain.Specifications.ISpecification<Permission>>())).Returns(Permissions());
            var MockMapper = new Mock<MapsterMapper.IMapper>();

            MockMapper.Setup(p => p.Map<PermissionDto>(It.IsAny<Permission>())).Returns(new PermissionDto());
            var service = new Mock<IPermissionQueryService>();
            service.Setup(p => p.Estructurar(It.IsAny<IEnumerable<PermissionDto>>())).Returns(new List<PermissionDto>());
            var token = new Mock<Domain.Service.ITokenService>();
            token.Setup(t => t.GetPermissions()).Returns(new List<Permission>());
            var Tenants = new Mock<Domain.Service.ITenantContext>();
            Tenants.SetupGet(t => t.IsPlatformAdmin).Returns(true);

            var instancia = new ListPermissionsHandler(MockMapper.Object, mockRepo.Object, service.Object, token.Object, Tenants.Object);
            var lista = instancia.ejecutar(new ListPermissions());

            Assert.IsInstanceOf<PermissionsResponse>(lista);
        }


        public IQueryable<Permission> Permissions() {
            var lista = new List<Permission>();
            var permisoModuloAutenticacion = new Permission { Id = getId(), Code = "administration", IsMenu = true, Name = "ADMINISTRACIÓN", SortOrder = 1, Url = "" };
            var permisoUsuario = new Permission { ParentPermissionId = permisoModuloAutenticacion.Id, Id = getId(), Code = "users", IsMenu = true, Name = "Lista Usuarios", SortOrder = 1, Url = "/users", Icon = "icon-people" };
            var permisoUsuarioCrear = new Permission { ParentPermissionId = permisoModuloAutenticacion.Id, Id = getId(), Code = "user-create", IsMenu = false, Name = "Crear usuario", SortOrder = 1, Url = "/users/crear" };
            var permisoRol = new Permission { ParentPermissionId = permisoModuloAutenticacion.Id, Id = getId(), Code = "roles", IsMenu = true, Name = "Lista roles", SortOrder = permisoUsuario.SortOrder + 1, Url = "/roles", Icon = "icon-key" };
            var permisoRolCrear = new Permission { ParentPermissionId = permisoModuloAutenticacion.Id, Id = getId(), Code = "role-create", IsMenu = false, Name = "Crear Roles", SortOrder = 1, Url = "/roles/crear" };
            lista.Add(permisoModuloAutenticacion);
            lista.Add(permisoUsuario);
            lista.Add(permisoUsuarioCrear);
            lista.Add(permisoRol);
            lista.Add(permisoRolCrear);
            return lista.AsQueryable();
        }

        private static int getId()
        {
            Id = Id + 1;
            return Id;
        }
    }
}
