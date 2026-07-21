using Aplicacion.Dtos;
using Aplicacion.Dtos.Importador;
using Aplicacion.Dtos.Usuario;
using Dominio.Models;
using Dominio.Repositories;
using Dominio.Repositories.Extensiones;
using Mapster;
using System.Collections.Generic;
using System.Linq;

namespace Aplicacion.Mappers
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Catalogo, DtoCatalogo>().TwoWays();
            config.NewConfig<Permiso, DtoPermiso>().TwoWays();
            config.NewConfig<UsuarioExterno, DtoImportador>().TwoWays();
            config.NewConfig<DtoTipoProducto, TipoProductoResponse>().TwoWays();
            config.NewConfig<UsuarioRegional, DtoUsuarioRegional>().TwoWays();
            config.NewConfig<UsuarioArea, DtoUsuarioArea>().TwoWays();

            config.NewConfig<Rol, DtoRol>()
                .Map(dest => dest.Permisos, src => MapPermisoIds(src.Permisos));
            config.NewConfig<DtoRol, Rol>()
                .Ignore(dest => dest.Permisos);

            config.NewConfig<DtoUsuario, Usuario>()
                .Ignore(dest => dest.Roles);
            config.NewConfig<DtoUsuarioResponse, Usuario>()
                .Ignore(dest => dest.Roles);

            config.NewConfig<IPagina<Rol>, RolPaginado>()
                .Map(dest => dest.Metadata, src => GetMetadata(src))
                .Map(dest => dest.valores, src => src.Select(r => r.Adapt<DtoRol>()).ToList());

            config.NewConfig<IPagina<Usuario>, DtoUsuariosPaginados>()
                .Map(dest => dest.Metadata, src => GetMetadata(src))
                .Map(dest => dest.valores, src => MapUsuariosPaginadosValores(src));

            config.NewConfig<IPagina<UsuarioExterno>, DtoImportadoresPaginados>()
                .Map(dest => dest.Metadata, src => GetMetadata(src))
                .Map(dest => dest.valores, src => src.Select(i => i.Adapt<DtoImportador>()).ToList());
        }

        private static IList<int> MapPermisoIds(IList<RolPermiso> permisos)
        {
            var permisosCol = new List<int>();
            if (permisos != null)
            {
                foreach (var item in permisos)
                {
                    permisosCol.Add(item.PermisoId);
                }
            }

            return permisosCol;
        }

        private static IEnumerable<DtoUsuarioResponse> MapUsuariosPaginadosValores(IEnumerable<Usuario> usuarios)
        {
            var lista = new List<DtoUsuarioResponse>();
            foreach (var item in usuarios)
            {
                var roles = new List<DtoRol>();
                if (item.Roles != null)
                {
                    foreach (var rol in item.Roles)
                    {
                        roles.Add(new DtoRol { Id = rol.Id, Nombre = rol.Rol.Nombre });
                    }
                }

                lista.Add(new DtoUsuarioResponse
                {
                    CambiarContrasena = item.CambiarContrasena,
                    Activo = item.Activo,
                    IdentificadorAcceso = item.IdentificadorAcceso,
                    Id = item.Id,
                    Nombre = item.Nombre,
                    DepartamentoId = item.DepartamentoId,
                    DepartamentoDescripcion = item.Departamento != null ? item.Departamento.Nombre : "",
                    Roles = roles,
                    FechaRegistro = item.FechaRegistro,
                    TipoUsuario = item.TipoUsuario
                });
            }

            return lista;
        }

        private static Metadata GetMetadata<T>(IPagina<T> paging)
        {
            return new Metadata
            {
                CurrentPage = paging.CurrentPage,
                HasPrevious = paging.HasPrevious,
                PageSize = paging.PageSize,
                TotalCount = paging.TotalCount,
                TotalPages = paging.TotalPages,
                HasNext = paging.HasNext
            };
        }
    }

    public static class UsuarioMappingHelper
    {
        public static DtoUsuarioResponse ToDtoResponse(Usuario usuario, IRolRepository rolRepository)
        {
            var roles = new List<DtoRol>();
            if (usuario.Roles != null)
            {
                foreach (var item in usuario.Roles)
                {
                    var rol = rolRepository.GetByIdConPermisos(item.RolId);
                    roles.Add(new DtoRol { Id = item.RolId, Descripcion = rol.Nombre });
                }
            }

            return new DtoUsuarioResponse
            {
                CambiarContrasena = usuario.CambiarContrasena,
                Activo = usuario.Activo,
                IdentificadorAcceso = usuario.IdentificadorAcceso,
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                DepartamentoId = usuario.DepartamentoId,
                DepartamentoDescripcion = usuario.Departamento != null ? usuario.Departamento.Nombre : "",
                Roles = roles,
                FechaRegistro = usuario.FechaRegistro,
                TipoUsuario = usuario.TipoUsuario
            };
        }

        public static DtoUsuarioLogin ToDtoLogin(Usuario usuario, IPermisoRepository permisoRepository)
        {
            var respuesta = new DtoUsuarioLogin
            {
                CambiarContrasena = usuario.CambiarContrasena,
                Activo = usuario.Activo,
                IdentificadorAcceso = usuario.IdentificadorAcceso,
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                DepartamentoId = usuario.DepartamentoId,
                DepartamentoNombre = usuario.Departamento != null ? usuario.Departamento.Nombre : null,
                FechaRegistro = usuario.FechaRegistro,
                TipoUsuario = usuario.TipoUsuario,
                Roles = MapLoginRoles(usuario, permisoRepository),
                UsuarioRegional = usuario.UsuarioRegional != null
                    ? usuario.UsuarioRegional.Select(r => r.Adapt<DtoUsuarioRegional>()).ToList()
                    : new List<DtoUsuarioRegional>(),
                UsuarioArea = usuario.UsuarioArea != null
                    ? usuario.UsuarioArea.Select(a => a.Adapt<DtoUsuarioArea>()).ToList()
                    : new List<DtoUsuarioArea>()
            };

            return respuesta;
        }

        private static IList<DtoRol> MapLoginRoles(Usuario usuario, IPermisoRepository permisoRepository)
        {
            var respuesta = new List<DtoRol>();
            if (usuario.IdentificadorAcceso == Usuario.correoUsuarioAdmin)
            {
                respuesta.Add(new DtoRol { Nombre = "Admin", PermisosConMetadata = GetAllPermisos(permisoRepository) });
                return respuesta;
            }

            if (usuario.Roles == null)
            {
                return respuesta;
            }

            foreach (var rol in usuario.Roles)
            {
                respuesta.Add(new DtoRol
                {
                    Id = rol.RolId,
                    Descripcion = rol.Rol.Nombre,
                    PermisosConMetadata = GetPermisos(rol.Rol.Permisos)
                });
            }

            return respuesta;
        }

        private static List<DtoPermiso> GetPermisos(IList<RolPermiso> permisos)
        {
            var respuesta = new List<DtoPermiso>();
            if (permisos == null)
            {
                return respuesta;
            }

            foreach (var permiso in permisos)
            {
                respuesta.Add(permiso.Permiso.Adapt<DtoPermiso>());
            }

            return respuesta;
        }

        private static List<DtoPermiso> GetAllPermisos(IPermisoRepository permisoRepository)
        {
            var respuesta = new List<DtoPermiso>();
            var lista = permisoRepository.GetAll().Where(c => Permiso.accesosParaAdmin.Contains(c.Id));
            foreach (var permiso in lista)
            {
                respuesta.Add(permiso.Adapt<DtoPermiso>());
            }

            return respuesta;
        }
    }
}
