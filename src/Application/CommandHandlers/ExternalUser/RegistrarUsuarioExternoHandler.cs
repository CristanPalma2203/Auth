using Application.Commands.ExternalUser;
using Application.Dtos;
using Domain.Helpers;
using Domain.Repositories;
using Domain.Service;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Application.CommandHandlers.ExternalUser
{
    public class RegisterExternalUserHandler : AbstractHandler<RegisterExternalUser>
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly IExternalUserRepository usuarioExternoRepository;
        private readonly IEmailHelper correoHelper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;

        public RegisterExternalUserHandler(
            IAppUserRepository appUserRepository,
            IExternalUserRepository usuarioExternoRepository,
            IEmailHelper correoHelper,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            this.appUserRepository = appUserRepository;
            this.usuarioExternoRepository = usuarioExternoRepository;
            this.correoHelper = correoHelper;
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
        }

        public override IResponse Handle(RegisterExternalUser message)
        {
            var correo = message.Email?.Trim();
            var nombreCompleto = string.IsNullOrWhiteSpace(message.LastName)
                ? message.Name?.Trim()
                : $"{message.Name?.Trim()} {message.LastName?.Trim()}".Trim();

            var appUser = new Domain.Models.AppUser
            {
                Password = message.Password,
                AccessIdentifier = correo,
                Name = nombreCompleto,
                DepartmentId = null
            };
            appUser.InitializeExternal(new List<int>());
            appUserRepository.Create(appUser);

            var perfil = new Domain.Models.ExternalUser
            {
                Name = message.Name?.Trim(),
                LastName = message.LastName?.Trim(),
                Email = correo,
                Identifier = correo,
                Phone = message.Phone?.Trim(),
                Mobile = message.Phone?.Trim()
            };
            perfil.RegisterAccount();
            usuarioExternoRepository.Create(perfil);

            unitOfWork.Save();

            var origen = message.Origen?.Trim().ToLowerInvariant();
            if (origen == "storefront" || origen == "tempora")
            {
                var baseUrl = configuration["AppSettings:VerifyEmailStorefront"]
                              ?? configuration["AppSettings:VerifyEmail"]
                              ?? "http://localhost:3001/verificar-correo";
                correoHelper.SendVerificationEmail(perfil.Email, perfil.VerificationToken, baseUrl);
            }
            else
            {
                correoHelper.SendVerificationEmail(perfil.Email, perfil.VerificationToken);
            }

            return new OkResponse();
        }
    }
}
