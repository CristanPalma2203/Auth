using Application.Commands.ExternalUser;
using Application.Dtos;
using Domain.Helpers;
using Domain.Repositories;
using Domain.Service;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Application.CommandHandlers.ExternalUser
{
    public class RegisterExternalUserHandler : AbstractHandler<RegisterExternalUser>
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly IExternalUserRepository usuarioExternoRepository;
        private readonly IEmailHelper correoHelper;
        private readonly IUnitOfWork unitOfWork;
        private readonly ITenantLookup tenantLookup;
        private readonly ILogger<RegisterExternalUserHandler> logger;

        public RegisterExternalUserHandler(
            IAppUserRepository appUserRepository,
            IExternalUserRepository usuarioExternoRepository,
            IEmailHelper correoHelper,
            IUnitOfWork unitOfWork,
            ITenantLookup tenantLookup,
            ILogger<RegisterExternalUserHandler> logger)
        {
            this.appUserRepository = appUserRepository;
            this.usuarioExternoRepository = usuarioExternoRepository;
            this.correoHelper = correoHelper;
            this.unitOfWork = unitOfWork;
            this.tenantLookup = tenantLookup;
            this.logger = logger;
        }

        public override IResponse Handle(RegisterExternalUser message)
        {
            var correo = message.Email?.Trim();
            var nombreCompleto = string.IsNullOrWhiteSpace(message.LastName)
                ? message.Name?.Trim()
                : $"{message.Name?.Trim()} {message.LastName?.Trim()}".Trim();

            var origen = message.Origen?.Trim().ToLowerInvariant();
            var tenantId = tenantLookup.ResolveIdByCode(origen);
            if (!tenantId.HasValue && !string.IsNullOrEmpty(origen))
            {
                logger.LogWarning(
                    "Registro externo sin TenantId: Origen={Origen} no mapea a tenant activo",
                    origen);
            }

            var appUser = new Domain.Models.AppUser
            {
                Password = message.Password,
                AccessIdentifier = correo,
                Name = nombreCompleto,
                DepartmentId = null,
                TenantId = tenantId
            };
            appUser.InitializeExternal(new List<int>());
            appUserRepository.Create(appUser);

            var perfil = new Domain.Models.ExternalUser
            {
                TenantId = tenantId,
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

            try
            {
                // URL/marca salen de dbo.tenant (StorefrontPublicUrl + brand); ERP usa VerifyEmail fallback.
                correoHelper.SendVerificationEmail(
                    perfil.Email,
                    perfil.VerificationToken,
                    verificarBaseUrl: null,
                    tenantId: tenantId);

                perfil.EmailSent = true;
                perfil.EmailSentAt = DateTime.Now;
                unitOfWork.Save();
            }
            catch (Exception ex)
            {
                // Cuenta ya creada; no tumbar registro por fallo Resend/SMTP
                logger.LogWarning(ex, "Registro ok pero fallo envio verificacion a {Email}", correo);
            }

            return new OkResponse();
        }
    }
}
