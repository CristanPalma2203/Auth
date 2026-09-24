using Application.CommandHandlers.Role;
using Application.Commands.Role;
using Application.Dtos;
using Application.Exceptions;
using Domain.Helpers;
using Domain.Models;
using Domain.Repositories;
using Domain.Service;
using MapsterMapper;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Auth.Tests
{
    public class CreateRoleHandlerTests
    {
        private const int ActorUserId = 7;
        private const int TemporaTenantId = 4;

        [Test]
        public void PlatformAdmin_can_assign_any_permission_to_new_tenant_role()
        {
            var handler = CreateHandler(
                isPlatformAdmin: true,
                callerTenantId: null,
                heldPermissionIds: new int[0],
                out var roles);

            var pedidos = 101;
            var ventas = 202;
            var reporteria = 303;
            var result = handler.Handle(Create(TemporaTenantId, pedidos, ventas, reporteria));

            Assert.That(result, Is.InstanceOf<RoleDto>());
            var persisted = Captured(roles);
            Assert.That(persisted.TenantId, Is.EqualTo(TemporaTenantId));
            Assert.That(persisted.IsAssignable, Is.True);
            Assert.That(persisted.Permissions.Select(p => p.PermissionId), Is.EquivalentTo(new[] { pedidos, ventas, reporteria }));
        }

        [Test]
        public void PlatformAdmin_without_tenant_still_cannot_create_a_platform_role()
        {
            var handler = CreateHandler(
                isPlatformAdmin: true,
                callerTenantId: null,
                heldPermissionIds: new int[0],
                out var roles);

            var ex = Assert.Throws<HttpException>(() => handler.Handle(Create(null, 50, 60)));

            Assert.That(ex.StatusCode, Is.EqualTo(422));
            Assert.That(ex.Message, Is.EqualTo("Debe seleccionar la empresa del rol"));
            roles.Verify(r => r.Create(It.IsAny<Role>()), Times.Never);
        }

        [Test]
        public void TenantUser_cannot_assign_permissions_they_do_not_hold()
        {
            var handler = CreateHandler(
                isPlatformAdmin: false,
                callerTenantId: TemporaTenantId,
                heldPermissionIds: new[] { 1 },
                out var roles);

            var ex = Assert.Throws<HttpException>(() => handler.Handle(Create(TemporaTenantId, 1, 101)));

            Assert.That(ex.StatusCode, Is.EqualTo(403));
            Assert.That(ex.Message, Is.EqualTo("Solo puede asignar permisos de los módulos contratados / que usted tiene"));
            roles.Verify(r => r.Create(It.IsAny<Role>()), Times.Never);
        }

        [Test]
        public void TenantUser_can_assign_permissions_they_hold()
        {
            var handler = CreateHandler(
                isPlatformAdmin: false,
                callerTenantId: TemporaTenantId,
                heldPermissionIds: new[] { 1, 8 },
                out var roles);

            handler.Handle(Create(TemporaTenantId, 1, 8));

            var persisted = Captured(roles);
            Assert.That(persisted.TenantId, Is.EqualTo(TemporaTenantId));
            Assert.That(persisted.Permissions.Select(p => p.PermissionId), Is.EquivalentTo(new[] { 1, 8 }));
        }

        private static CreateRole Create(int? tenantId, params int[] permissionIds)
        {
            return new CreateRole
            {
                Role = new RoleDto
                {
                    Name = "Tempora",
                    Description = "Rol empresa",
                    TenantId = tenantId,
                    PermissionIds = permissionIds.ToList()
                }
            };
        }

        private static Role Captured(Mock<IRoleRepository> roles)
        {
            return (Role)roles.Invocations.Single(i => i.Method.Name == "Create").Arguments[0];
        }

        private static CreateRoleHandler CreateHandler(
            bool isPlatformAdmin,
            int? callerTenantId,
            int[] heldPermissionIds,
            out Mock<IRoleRepository> roles)
        {
            roles = new Mock<IRoleRepository>();
            roles.Setup(r => r.Create(It.IsAny<Role>())).Returns<Role>(role => role);

            var tenants = new Mock<ITenantRepository>();
            tenants.Setup(t => t.GetById(TemporaTenantId)).Returns(new Tenant
            {
                Id = TemporaTenantId,
                Name = "Tempora",
                IsActive = true
            });

            var users = new Mock<IAppUserRepository>();
            users.Setup(u => u.GetById(ActorUserId)).Returns(new AppUser { Id = ActorUserId, Name = "Alex" });

            var token = new Mock<ITokenService>();
            token.Setup(t => t.GetUserId()).Returns(ActorUserId);
            token.Setup(t => t.GetPermissions()).Returns(
                heldPermissionIds.Select(id => new Permission { Id = id }).ToList());

            var tenantContext = new Mock<ITenantContext>();
            tenantContext.SetupGet(t => t.IsPlatformAdmin).Returns(isPlatformAdmin);
            tenantContext.SetupGet(t => t.TenantId).Returns(callerTenantId);

            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<Role>(It.IsAny<RoleDto>())).Returns(() => new Role());
            mapper.Setup(m => m.Map<RoleDto>(It.IsAny<Role>())).Returns(new RoleDto());

            return new CreateRoleHandler(
                roles.Object,
                tenants.Object,
                mapper.Object,
                Mock.Of<IEmailHelper>(),
                token.Object,
                users.Object,
                tenantContext.Object);
        }
    }
}
