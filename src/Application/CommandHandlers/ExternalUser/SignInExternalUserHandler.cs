using Application.Commands.ExternalUser;
using Application.Dtos;
using Application.Dtos.AppUser;
using Application.Exceptions;
using Application.Mappers;
using Domain.Models;
using Domain.Repositories;
using Domain.Service;
using Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.CommandHandlers.ExternalUser
{
    public class SignInExternalUserHandler : AbstractHandler<SignInExternalUser>
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly IExternalUserRepository externalUserRepository;
        private readonly IPermissionRepository permissionRepository;
        private readonly ITokenService tokenService;

        public SignInExternalUserHandler(
            IAppUserRepository appUserRepository,
            IExternalUserRepository externalUserRepository,
            IPermissionRepository permissionRepository,
            ITokenService tokenService)
        {
            this.appUserRepository = appUserRepository;
            this.externalUserRepository = externalUserRepository;
            this.permissionRepository = permissionRepository;
            this.tokenService = tokenService;
        }

        public override IResponse Handle(SignInExternalUser message)
        {
            var identifier = message?.Identifier;
            if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrEmpty(message.Password))
            {
                throw new HttpException(422, StorefrontLoginMessages.InvalidCredentials);
            }

            var appUser = appUserRepository.GetUserWithRolePermissions(
                new FindUserByIdentifierAndPassword(identifier, message.Password));

            if (appUser == null || string.IsNullOrEmpty(appUser.Password))
            {
                throw new HttpException(422, StorefrontLoginMessages.InvalidCredentials);
            }

            if (!string.Equals(appUser.UserType, Domain.Models.AppUser.externalUserType, StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpException(403, StorefrontLoginMessages.InternalUserNotAllowed);
            }

            var profile = externalUserRepository
                .Filter(new FindExternalUserByIdentifier(identifier))
                .FirstOrDefault();

            if (profile == null)
            {
                throw new HttpException(403, StorefrontLoginMessages.InternalUserNotAllowed);
            }

            if (!profile.EmailVerified)
            {
                throw new HttpException(422, StorefrontLoginMessages.EmailNotVerified);
            }

            if (!profile.AccessApproved)
            {
                throw new HttpException(422, StorefrontLoginMessages.AccessNotApproved);
            }

            appUser.Roles ??= new List<UserRole>();
            var respuesta = UserMappingHelper.ToDtoLogin(appUser, permissionRepository);
            respuesta.Token = tokenService.CreateOrGetToken(appUser);
            return respuesta;
        }
    }
}
