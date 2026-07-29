using Application.Commands.ExternalUser;
using Application.Dtos;
using Application.Dtos.ExternalUser;
using MapsterMapper;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.CommandHandlers.ExternalUser
{
    public class GetExternalUserByIdentifierHandler : AbstractHandler<GetExternalUserByIdentifier>
    {
        private readonly IExternalUserRepository externalUserRepository;
        private readonly IMapper mapper;

        public GetExternalUserByIdentifierHandler(IExternalUserRepository externalUserRepository, IMapper mapper)
        {
            this.externalUserRepository = externalUserRepository;
            this.mapper = mapper;
        }

        public override IResponse Handle(GetExternalUserByIdentifier message)
        {
            var externalUser = externalUserRepository.Set().AsNoTracking().
                Include(c => c.Department).Include(c => c.Municipality).Include(c => c.Nationality)
                .FirstOrDefault(c => c.Identifier == message.IdImportador && c.AccessApproved== true);
            if (externalUser !=null) return mapper.Map<ExternalUserDto>(externalUser);
            return new ExternalUserDto();
        }
    }
}
