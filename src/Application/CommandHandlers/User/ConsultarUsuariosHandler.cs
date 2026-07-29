using Application.Commands.AppUser;
using Application.Dtos;
using Application.Dtos.AppUser;
using Domain.Specifications;
using Domain.Repositories;
using Domain.Repositories.Extensiones;
using Domain.Service;
using MapsterMapper;
using System.Linq;

namespace Application.CommandHandlers.AppUser
{
    class ListUsersHandler : AbstractHandler<ListUsers>
    {
        private readonly IAppUserRepository usersRepository;
        private readonly IMapper mapper;
        private readonly ITenantContext tenantContext;

        public ListUsersHandler(
            IAppUserRepository usersRepository,
            IMapper mapper,
            ITenantContext tenantContext)
        {
            this.usersRepository = usersRepository;
            this.mapper = mapper;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(ListUsers message)
        {
            IPagina<Domain.Models.AppUser> respuesta;
            var tenantSpec = new FindUsersByTenant(tenantContext.TenantId, tenantContext.IsPlatformAdmin);

            if (!string.IsNullOrWhiteSpace(message.Name) || !string.IsNullOrWhiteSpace(message.correo) || message.idDepartamento != 0)
            {
                var nameSpec = new FindUserByNameAndEmail(message.Name, message.correo, message.idDepartamento);
                respuesta = usersRepository.GetPagedWithRole(message, new SpecAnd<Domain.Models.AppUser>(nameSpec, tenantSpec));
            }
            else
            {
                respuesta = usersRepository.GetPagedWithRole(message, tenantSpec);
            }

            return mapper.Map<UsersPagedDto>(respuesta);
        }
    }

    /// <summary>Combina dos especificaciones con AND.</summary>
    internal class SpecAnd<T> : ISpecification<T>
    {
        private readonly ISpecification<T> a;
        private readonly ISpecification<T> b;

        public SpecAnd(ISpecification<T> a, ISpecification<T> b)
        {
            this.a = a;
            this.b = b;
        }

        public System.Func<T, bool> Traer()
        {
            var fa = a.Traer();
            var fb = b.Traer();
            return x => fa(x) && fb(x);
        }
    }
}
