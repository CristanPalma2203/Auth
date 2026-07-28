using Aplicacion.Commands.Usuario;
using Aplicacion.Dtos;
using Aplicacion.Dtos.Usuario;
using Dominio.Especificaciones;
using Dominio.Repositories;
using Dominio.Repositories.Extensiones;
using Dominio.Service;
using MapsterMapper;
using System.Linq;

namespace Aplicacion.CommandHandlers.Usuario
{
    class ConsultarUsuariosHandler : AbstractHandler<ConsultarUsuarios>
    {
        private readonly IUsuarioRepository usuariosRepository;
        private readonly IMapper mapper;
        private readonly ITenantContext tenantContext;

        public ConsultarUsuariosHandler(
            IUsuarioRepository usuariosRepository,
            IMapper mapper,
            ITenantContext tenantContext)
        {
            this.usuariosRepository = usuariosRepository;
            this.mapper = mapper;
            this.tenantContext = tenantContext;
        }

        public override IResponse Handle(ConsultarUsuarios message)
        {
            IPagina<Dominio.Models.Usuario> respuesta;
            var tenantSpec = new BuscarUsuariosPorTenant(tenantContext.TenantId, tenantContext.IsPlatformAdmin);

            if (!string.IsNullOrWhiteSpace(message.Name) || !string.IsNullOrWhiteSpace(message.correo) || message.idDepartamento != 0)
            {
                var nameSpec = new BuscarUsuarioPorNombreYCorreo(message.Name, message.correo, message.idDepartamento);
                respuesta = usuariosRepository.ConsultarPaginadoConRol(message, new SpecAnd<Dominio.Models.Usuario>(nameSpec, tenantSpec));
            }
            else
            {
                respuesta = usuariosRepository.ConsultarPaginadoConRol(message, tenantSpec);
            }

            return mapper.Map<DtoUsuariosPaginados>(respuesta);
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
