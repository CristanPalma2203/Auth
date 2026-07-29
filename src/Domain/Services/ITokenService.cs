using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Service
{
   public interface ITokenService
    {
        string CreateOrGetToken(AppUser appUser);
        string GetExistingToken(int userId);
        void RemoveToken();
        string GetTokenFromRequest();
        bool VerifyToken();

        string GetUserIdentifier();
        int GetUserId();
        int? GetTenantId();
        string GetTenantCodigo();

        List<Permission> GetPermissions();
    }
}
