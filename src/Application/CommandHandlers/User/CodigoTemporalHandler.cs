using Application.Commands.AppUser;
using Application.Dtos;
using Domain.Helpers;
using Domain.Repositories;
using Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.CommandHandlers.AppUser
{
    public class CodigoTemporalHandler : AbstractHandler<TemporaryCode>
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly IEmailHelper correoHelper;
        private readonly IExternalUserRepository externalUserRepository;

        public CodigoTemporalHandler(
            IEmailHelper correoHelper,
            IExternalUserRepository externalUserRepository,
            IAppUserRepository appUserRepository)
        {
            this.appUserRepository = appUserRepository;
            this.correoHelper = correoHelper;
            this.externalUserRepository = externalUserRepository;
        }

        public override IResponse Handle(TemporaryCode message)
        {
            var access = message.AccessIdentifier?.Trim();
            if (string.IsNullOrWhiteSpace(access))
                return new OkResponse();

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var code = new string(
                Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());

            var solicitud = appUserRepository
                .GetAll()
                .FirstOrDefault(x =>
                    x.AccessIdentifier != null
                    && x.AccessIdentifier.ToLower().Trim() == access.ToLower());

            if (solicitud == null)
                return new OkResponse();

            solicitud.TemporaryCode = code;
            appUserRepository.Update(solicitud.Id, solicitud);

            var destinos = new List<string>();
            if (solicitud.UserType == "external-user")
            {
                var externalUser = externalUserRepository
                    .Filter(new FindExternalUserByIdentifier(access))
                    .FirstOrDefault()
                    ?? externalUserRepository
                        .Filter(c =>
                            c.Email != null
                            && c.Email.ToLower().Trim() == access.ToLower())
                        .FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(externalUser?.Email))
                    destinos.Add(externalUser.Email);
                else
                    destinos.Add(solicitud.AccessIdentifier);
            }
            else
            {
                destinos.Add(solicitud.AccessIdentifier);
            }

            correoHelper.SendRequestUpdateEmail(destinos, "Codigo de verificacion: ", code);
            return new OkResponse();
        }
    }
}
