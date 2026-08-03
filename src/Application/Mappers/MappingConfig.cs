using Application.Dtos;
using Application.Dtos.ExternalUser;
using Application.Dtos.AppUser;
using Domain.Models;
using Domain.Repositories;
using Domain.Repositories.Extensiones;
using Mapster;
using System.Collections.Generic;
using System.Linq;

namespace Application.Mappers
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Catalog, CatalogDto>().TwoWays();
            config.NewConfig<Permission, PermissionDto>().TwoWays();
            config.NewConfig<ExternalUser, ExternalUserDto>().TwoWays();
            config.NewConfig<ProductTypeDto, TipoProductoResponse>().TwoWays();
            config.NewConfig<UserRegional, UserRegionalDto>().TwoWays();
            config.NewConfig<UserArea, UserAreaDto>().TwoWays();

            config.NewConfig<Role, RoleDto>()
                .Map(dest => dest.PermissionIds, src => MapPermissionIds(src.Permissions));
            config.NewConfig<RoleDto, Role>()
                .Ignore(dest => dest.Permissions);

            config.NewConfig<UserDto, AppUser>()
                .Ignore(dest => dest.Roles);
            config.NewConfig<DtoUsuarioResponse, AppUser>()
                .Ignore(dest => dest.Roles);

            config.NewConfig<IPagina<Role>, RolesPagedDto>()
                .Map(dest => dest.Metadata, src => GetMetadata(src))
                .Map(dest => dest.Values, src => src.Select(r => r.Adapt<RoleDto>()).ToList());

            config.NewConfig<IPagina<AppUser>, UsersPagedDto>()
                .Map(dest => dest.Metadata, src => GetMetadata(src))
                .Map(dest => dest.Values, src => MapPagedUsersValues(src));

            config.NewConfig<IPagina<ExternalUser>, ExternalUsersPagedDto>()
                .Map(dest => dest.Metadata, src => GetMetadata(src))
                .Map(dest => dest.Values, src => src.Select(i => i.Adapt<ExternalUserDto>()).ToList());
        }

        private static IList<int> MapPermissionIds(IList<RolePermission> permisos)
        {
            var permisosCol = new List<int>();
            if (permisos != null)
            {
                foreach (var item in permisos)
                {
                    permisosCol.Add(item.PermissionId);
                }
            }

            return permisosCol;
        }

        private static IEnumerable<DtoUsuarioResponse> MapPagedUsersValues(IEnumerable<AppUser> users)
        {
            var lista = new List<DtoUsuarioResponse>();
            foreach (var item in users)
            {
                var roles = new List<RoleDto>();
                if (item.Roles != null)
                {
                    foreach (var Roles in item.Roles)
                    {
                        roles.Add(new RoleDto { Id = Roles.Id, Name = Roles.Role.Name });
                    }
                }

                lista.Add(new DtoUsuarioResponse
                {
                    MustChangePassword = item.MustChangePassword,
                    IsActive = item.IsActive,
                    AccessIdentifier = item.AccessIdentifier,
                    Id = item.Id,
                    Name = item.Name,
                    DepartmentId = item.DepartmentId,
                    DepartamentoDescripcion = item.Department != null ? item.Department.Name : "",
                    Roles = roles,
                    RegisteredAt = item.RegisteredAt,
                    UserType = item.UserType
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

    public static class UserMappingHelper
    {
        public static DtoUsuarioResponse ToDtoResponse(AppUser appUser, IRoleRepository roleRepository)
        {
            var roles = new List<RoleDto>();
            if (appUser.Roles != null)
            {
                foreach (var item in appUser.Roles)
                {
                    var Roles = roleRepository.GetByIdWithPermissions(item.RoleId);
                    roles.Add(new RoleDto { Id = item.RoleId, Description = Roles.Name });
                }
            }

            return new DtoUsuarioResponse
            {
                MustChangePassword = appUser.MustChangePassword,
                IsActive = appUser.IsActive,
                AccessIdentifier = appUser.AccessIdentifier,
                Id = appUser.Id,
                Name = appUser.Name,
                DepartmentId = appUser.DepartmentId,
                DepartamentoDescripcion = appUser.Department != null ? appUser.Department.Name : "",
                Roles = roles,
                RegisteredAt = appUser.RegisteredAt,
                UserType = appUser.UserType
            };
        }

        public static UserLoginDto ToDtoLogin(AppUser appUser, IPermissionRepository permissionRepository)
        {
            var respuesta = new UserLoginDto
            {
                MustChangePassword = appUser.MustChangePassword,
                IsActive = appUser.IsActive,
                AccessIdentifier = appUser.AccessIdentifier,
                Id = appUser.Id,
                Name = appUser.Name,
                DepartmentId = appUser.DepartmentId,
                DepartamentoNombre = appUser.Department != null ? appUser.Department.Name : null,
                RegisteredAt = appUser.RegisteredAt,
                UserType = appUser.UserType,
                TenantId = appUser.TenantId,
                TenantCodigo = appUser.Tenant?.Code,
                ProfileFileId = appUser.ProfileFileId,
                Roles = MapLoginRoles(appUser, permissionRepository),
                UserRegional = appUser.UserRegional != null
                    ? appUser.UserRegional.Select(r => r.Adapt<UserRegionalDto>()).ToList()
                    : new List<UserRegionalDto>(),
                UserArea = appUser.UserArea != null
                    ? appUser.UserArea.Select(a => a.Adapt<UserAreaDto>()).ToList()
                    : new List<UserAreaDto>()
            };

            return respuesta;
        }

        private static IList<RoleDto> MapLoginRoles(AppUser appUser, IPermissionRepository permissionRepository)
        {
            var respuesta = new List<RoleDto>();
            if (appUser.AccessIdentifier == AppUser.adminUserEmail)
            {
                respuesta.Add(new RoleDto { Name = "Admin", PermissionsWithMetadata = GetAllPermissionDtos(permissionRepository) });
                return respuesta;
            }

            if (appUser.Roles == null)
            {
                return respuesta;
            }

            foreach (var Roles in appUser.Roles)
            {
                respuesta.Add(new RoleDto
                {
                    Id = Roles.RoleId,
                    Description = Roles.Role.Name,
                    PermissionsWithMetadata = BuildPermissionDtos(Roles.Role.Permissions)
                });
            }

            return respuesta;
        }

        private static List<PermissionDto> BuildPermissionDtos(IList<RolePermission> permisos)
        {
            var respuesta = new List<PermissionDto>();
            if (permisos == null)
            {
                return respuesta;
            }

            foreach (var Permissions in permisos)
            {
                respuesta.Add(Permissions.Permission.Adapt<PermissionDto>());
            }

            return respuesta;
        }

        private static List<PermissionDto> GetAllPermissionDtos(IPermissionRepository permissionRepository)
        {
            var respuesta = new List<PermissionDto>();
            var lista = permissionRepository.GetAll().Where(c => Permission.accesosParaAdmin.Contains(c.Id));
            foreach (var Permissions in lista)
            {
                respuesta.Add(Permissions.Adapt<PermissionDto>());
            }

            return respuesta;
        }
    }
}
