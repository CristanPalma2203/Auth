using Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Services.PermissionQuery
{
    public class PermissionQueryService : IPermissionQueryService
    {
        public IEnumerable<PermissionDto> Estructurar(IEnumerable<PermissionDto> permissions)
        {
            var padres = permissions.Where(c=>c.ParentPermissionId==null);
            foreach (var permission in padres)getHijos(permission,permissions);
            return padres;
        }

        private void getHijos(PermissionDto permission,IEnumerable<PermissionDto> allPermissions) {

            var children = allPermissions.Where(c=>c.ParentPermissionId==permission.Id);
            if (children == null || children.Count() == 0) return;
            foreach (var child in children)getHijos(child, allPermissions);
            permission.Children = children;
        }
    }
}
