using Application.Commands.ExternalUser;
using Application.Dtos;
using Application.Dtos.ExternalUser;
using MapsterMapper;
using Domain.Specifications;
using Domain.Repositories;
using Domain.Repositories.Extensiones;

namespace Application.CommandHandlers.ExternalUser
{
    public class ListExternalUsersHandler : AbstractHandler<ListExternalUsers>
    {
        private readonly IExternalUserRepository externalUserRepository;
        private readonly IMapper mapper;

        public ListExternalUsersHandler(IExternalUserRepository externalUserRepository, IMapper mapper) {
            this.externalUserRepository = externalUserRepository;
            this.mapper = mapper;
        }
        public override IResponse Handle(ListExternalUsers message)
        {
            IPagina<Domain.Models.ExternalUser> respuesta;
            if (!string.IsNullOrWhiteSpace(message.identifier ) || !string.IsNullOrWhiteSpace(message.nombre) )
            {
                respuesta = externalUserRepository.GetPaged(message, new FindExternalUserByIdentifierAndName(message.identifier, message.nombre));
            }
            else
            {
                var consulta = !string.IsNullOrWhiteSpace(message.Consulta) ? message.Consulta : "AccessApprovedAt = null AND EmailVerified=true"; //"AccessApprovedAt = null AND FechaDenegacionAcceso = null AND EmailVerified=true";
                respuesta = externalUserRepository.Filter(message, consulta);
            }
           
           return mapper.Map<ExternalUsersPagedDto>(respuesta); 
        }
    }
}
