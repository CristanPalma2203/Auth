using Application.Commands.ExternalUser;
using Application.Dtos;
using Application.Dtos.ExternalUser;
using MapsterMapper;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Specifications;

namespace Application.CommandHandlers.ExternalUser
{
    public class GetExternalUserByUserIdHandler : AbstractHandler<GetExternalUserByUserId>
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly IExternalUserRepository externalUserRepository;
        private readonly IMapper mapper;


        public GetExternalUserByUserIdHandler(IExternalUserRepository externalUserRepository, IMapper mapper, IAppUserRepository appUserRepository)
        {
            this.appUserRepository = appUserRepository;
            this.externalUserRepository = externalUserRepository;
            this.mapper = mapper;
        }

        public override IResponse Handle(GetExternalUserByUserId message)
        {
            var appUser = appUserRepository.GetById(message.IdUsuario);
            var externalUser = externalUserRepository.Filter(new FindExternalUserByIdentifier(appUser.AccessIdentifier)).FirstOrDefault();
            if (externalUser != null) return mapper.Map<ExternalUserDto>(externalUser);
            return new ExternalUserDto();
        }
    }
}
