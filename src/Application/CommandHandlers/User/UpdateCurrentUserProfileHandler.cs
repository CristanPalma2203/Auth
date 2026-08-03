using System;
using Application.Commands.AppUser;
using Application.Dtos;
using Application.Dtos.AppUser;
using Application.Exceptions;
using Application.Mappers;
using Domain.Repositories;
using Domain.Service;

namespace Application.CommandHandlers.AppUser
{
    public class UpdateCurrentUserProfileHandler : AbstractHandler<UpdateCurrentUserProfile>
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly IPermissionRepository permissionRepository;
        private readonly ITokenService tokenService;

        public UpdateCurrentUserProfileHandler(
            IAppUserRepository appUserRepository,
            IPermissionRepository permissionRepository,
            ITokenService tokenService)
        {
            this.appUserRepository = appUserRepository;
            this.permissionRepository = permissionRepository;
            this.tokenService = tokenService;
        }

        public override IResponse Handle(UpdateCurrentUserProfile message)
        {
            var id = tokenService.GetUserId();
            var appUser = appUserRepository.GetById(id);
            if (appUser == null)
                throw new HttpException(404, "AppUser no encontrado");

            appUser.ProfileFileId = message.ProfileFileId;
            appUser.UpdatedAt = DateTime.Now;
            appUserRepository.Update(appUser.Id, appUser);

            var withRoles = appUserRepository.GetUserWithRolePermissions(
                new Domain.Specifications.FindUserById(id));
            return UserMappingHelper.ToDtoLogin(withRoles, permissionRepository);
        }
    }
}
