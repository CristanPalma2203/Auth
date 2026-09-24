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
    public class EditRoleHandlerTests
    {
        private const int ActorUserId = 7;
        private const int TemporaRoleId = 2;
        private const int TemporaTenantId = 4;

        [Test]
        public void PlatformAdmin_can_assign_any_permission_to_tenant_role()
        {
            var role = TenantRole(TemporaRoleId, TemporaTenantId, existingPermissionId: 1);
            var handler = CreateHandler(role, isPlatformAdmin: true, actorRoleIds: new[] { 99 }, heldPermissionIds: new int[0]);

            var pedidos = 101;
            var ventas = 202;
            var reporteria = 303;
            var result = handler.Handle(Edit(TemporaRoleId, pedidos, ventas, reporteria));

            Assert.That(result, Is.InstanceOf<RoleDto>());
            Assert.That(role.Permissions.Select(p => p.PermissionId), Is.EquivalentTo(new[] { pedidos, ventas, reporteria }));
            Assert.That(role.Name, Is.EqualTo("Tempora"));
        }

        [Test]
        public void PlatformAdmin_editing_platform_role_still_accepts_any_permission()
        {
            var role = new Role
            {
                Id = 1,
                TenantId = null,
                Name = "Platform",
                Permissions = new List<RolePermission>()
            };
            var handler = CreateHandler(role, isPlatformAdmin: true, actorRoleIds: new[] { 99 }, heldPermissionIds: new int[0]);

            handler.Handle(Edit(1, 50, 60));

            Assert.That(role.Permissions.Select(p => p.PermissionId), Is.EquivalentTo(new[] { 50, 60 }));
            Assert.That(role.TenantId, Is.Null);
        }

        [Test]
        public void TenantUser_cannot_assign_permissions_they_do_not_hold()
        {
            var role = TenantRole(TemporaRoleId, TemporaTenantId, existingPermissionId: 1);
            var handler = CreateHandler(role, isPlatformAdmin: false, actorRoleIds: new[] { 99 }, heldPermissionIds: new[] { 1 });

            var ex = Assert.Throws<HttpException>(() => handler.Handle(Edit(TemporaRoleId, 1, 101)));

            Assert.That(ex.StatusCode, Is.EqualTo(403));
            Assert.That(ex.Message, Is.EqualTo("Solo puede asignar permisos de los módulos contratados / que usted tiene"));
            Assert.That(role.Permissions.Select(p => p.PermissionId), Is.EquivalentTo(new[] { 1 }));
        }

        [Test]
        public void TenantUser_can_assign_permissions_they_hold()
        {
            var role = TenantRole(TemporaRoleId, TemporaTenantId, existingPermissionId: 1);
            var handler = CreateHandler(role, isPlatformAdmin: false, actorRoleIds: new[] { 99 }, heldPermissionIds: new[] { 1, 8 });

            handler.Handle(Edit(TemporaRoleId, 1, 8));

            Assert.That(role.Permissions.Select(p => p.PermissionId), Is.EquivalentTo(new[] { 1, 8 }));
        }

        [Test]
        public void Editing_the_callers_own_role_stays_forbidden()
        {
            var role = TenantRole(TemporaRoleId, TemporaTenantId, existingPermissionId: 1);
            var handler = CreateHandler(role, isPlatformAdmin: true, actorRoleIds: new[] { TemporaRoleId }, heldPermissionIds: new int[0]);

            var ex = Assert.Throws<HttpException>(() => handler.Handle(Edit(TemporaRoleId, 101)));

            Assert.That(ex.StatusCode, Is.EqualTo(403));
            Assert.That(ex.Message, Is.EqualTo("No puede editar el rol que tiene asignado. Solo puede verlo."));
            Assert.That(role.Permissions.Select(p => p.PermissionId), Is.EquivalentTo(new[] { 1 }));
        }

        private static Role TenantRole(int roleId, int tenantId, int existingPermissionId)
        {
            return new Role
            {
                Id = roleId,
                TenantId = tenantId,
                Name = "Antes",
                Permissions = new List<RolePermission>
                {
                    new RolePermission { Id = 10, RoleId = roleId, PermissionId = existingPermissionId }
                }
            };
        }

        private static EditRole Edit(int roleId, params int[] permissionIds)
        {
            return new EditRole
            {
                Id = roleId,
                Role = new RoleDto
                {
                    Id = roleId,
                    Name = "Tempora",
                    Description = "Rol empresa",
                    PermissionIds = permissionIds.ToList()
                }
            };
        }

        private static EditRoleHandler CreateHandler(Role role, bool isPlatformAdmin, int[] actorRoleIds, int[] heldPermissionIds)
        {
            var roles = new Mock<IRoleRepository>();
            roles.Setup(r => r.GetByIdWithPermissions(role.Id)).Returns(role);

            var users = new Mock<IAppUserRepository>();
            users.Setup(u => u.GetByIdConRoles(ActorUserId)).Returns(new AppUser
            {
                Id = ActorUserId,
                Name = "Alex",
                Roles = actorRoleIds.Select(id => new UserRole { RoleId = id, UserId = ActorUserId }).ToList()
            });

            var token = new Mock<ITokenService>();
            token.Setup(t => t.GetUserId()).Returns(ActorUserId);
            token.Setup(t => t.GetPermissions()).Returns(
                heldPermissionIds.Select(id => new Permission { Id = id }).ToList());

            var tenant = new Mock<ITenantContext>();
            tenant.SetupGet(t => t.IsPlatformAdmin).Returns(isPlatformAdmin);

            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<RoleDto>(It.IsAny<Role>())).Returns(new RoleDto());

            return new EditRoleHandler(
                roles.Object,
                users.Object,
                token.Object,
                Mock.Of<IEmailHelper>(),
                mapper.Object,
                Mock.Of<IRolePermissionRepository>(),
                tenant.Object);
        }
    }
}
