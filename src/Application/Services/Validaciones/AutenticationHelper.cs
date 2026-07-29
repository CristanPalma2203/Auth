using Application.Exceptions;
using Domain.Models;
using Domain.Service;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Validaciones
{
    public class AutenticationHelper : IAutenticationHelper
    {
        private readonly ITokenService tokeService;

        public AutenticationHelper(ITokenService tokeService)
        {
            this.tokeService = tokeService;
        }
        public void Autenticado(IList<string> permisos)
        {
            if (permisos.Count == 0) return;
     
            if (string.IsNullOrWhiteSpace(tokeService.GetTokenFromRequest())) {
                throw new HttpException(401, "Unauthorized");
            }
            var respuesta = tokeService.VerifyToken();
            if (respuesta)
            {
                if (tokeService.GetUserIdentifier()==AppUser.adminUserEmail) return;
                if(!SearchInCollections(permisos, tokeService.GetPermissions())) throw new HttpException(403, "Forbidden");
            }
            else {
                throw new HttpException(401, "Unauthorized");
            }
        }
        private bool SearchInCollections(IList<string>  ListaPermisos, List<Permission> permisosToken) {

            var encuentra = false;
            foreach (var item in ListaPermisos)
            {
                var resultado = permisosToken.Where(c => c.Code == item).FirstOrDefault();
                if (resultado != null) {
                    encuentra = true;
                }
            }
            return encuentra;
        }
    }
}
