using Application.Commands.AppUser;
using Application.Dtos;
using Application.Exceptions;
using AppUserModel = Domain.Models.AppUser;
using Domain.Repositories;
using Domain.Service;
using System;
using System.Linq;

namespace Application.CommandHandlers.AppUser
{
    public class DeleteUserHandler : AbstractHandler<DeleteUser>
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly IUsuarioRolRepository userRoleRepository;
        private readonly IExternalUserRepository externalUserRepository;
        private readonly ITenantContext tenantContext;
        private readonly ITokenService tokenService;
        private readonly IUnitOfWork unitOfWork;

        public DeleteUserHandler(
            IAppUserRepository appUserRepository,
            IUsuarioRolRepository userRoleRepository,
            IExternalUserRepository externalUserRepository,
            ITenantContext tenantContext,
            ITokenService tokenService,
            IUnitOfWork unitOfWork)
        {
            this.appUserRepository = appUserRepository;
            this.userRoleRepository = userRoleRepository;
            this.externalUserRepository = externalUserRepository;
            this.tenantContext = tenantContext;
            this.tokenService = tokenService;
            this.unitOfWork = unitOfWork;
        }

        public override IResponse Handle(DeleteUser message)
        {
            var actorId = tokenService.GetUserId();
            if (message.Id == actorId)
                throw new HttpException(422, "No podés eliminar tu propio usuario");

            if (message.Id == AppUserModel.AdminUserId)
                throw new HttpException(422, "No se puede eliminar el administrador de la plataforma");

            var dbUser = appUserRepository.GetByIdConRoles(message.Id);
            if (dbUser == null)
                throw new HttpException(404, "Usuario no encontrado");

            tenantContext.EnsureSameTenantOrPlatform(dbUser.TenantId);

            if (string.Equals(dbUser.AccessIdentifier, AppUserModel.adminUserEmail, StringComparison.OrdinalIgnoreCase))
                throw new HttpException(422, "No se puede eliminar el administrador de la plataforma");

            var ligados = externalUserRepository.GetAll()
                .Where(e => e.ManagedByUserId == message.Id)
                .ToList();
            foreach (var ext in ligados)
            {
                ext.ManagedByUserId = null;
                externalUserRepository.Update(ext.Id, ext);
            }

            if (dbUser.Roles != null)
            {
                foreach (var role in dbUser.Roles.ToList())
                    userRoleRepository.Delete(role.Id);
            }

            appUserRepository.Delete(message.Id);

            try
            {
                unitOfWork.Save();
            }
            catch (Exception ex) when (IsForeignKey(ex))
            {
                throw new HttpException(422, "No se puede eliminar: hay registros ligados a este usuario");
            }

            return new OkResponse();
        }

        private static bool IsForeignKey(Exception ex)
        {
            for (var cur = ex; cur != null; cur = cur.InnerException)
            {
                var text = cur.Message ?? string.Empty;
                if (text.IndexOf("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) >= 0
                    || text.IndexOf("REFERENCE constraint", StringComparison.OrdinalIgnoreCase) >= 0
                    || text.IndexOf("conflicted with the REFERENCE", StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }
    }
}
