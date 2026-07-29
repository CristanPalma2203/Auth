using Application.Commands.AppUser;
using Application.Dtos;
using Domain.Specifications;
using Domain.Helpers;
using Domain.Repositories;
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
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());

            var resp = result;
            var solicitud = appUserRepository.GetAll().FirstOrDefault(x => x.AccessIdentifier == message.AccessIdentifier);
            if (solicitud != null)
            {
                solicitud.TemporaryCode = resp;
                appUserRepository.Update(solicitud.Id, solicitud);
                var motivo = "Code de Verificaci?n: ".ToString();
                var lista = new List<string>();
                if (solicitud.UserType == "external-user")
                {
                    var externalUser = externalUserRepository.Filter(new FindExternalUserByIdentifier(message.AccessIdentifier)).FirstOrDefault();
                    if (solicitud != null)
                    {
                        lista.Add(externalUser.Email);
                    }
                }
                else
                {
                    if (solicitud != null)
                    {
                        lista.Add(solicitud.AccessIdentifier);
                    }
                }

                correoHelper.SendRequestUpdateEmail(lista, motivo, resp);
            }

            return new OkResponse();
        }
    }
}
