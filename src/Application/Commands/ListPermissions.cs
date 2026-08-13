using Application.Common;
using Application.Dtos;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands
{
    public class ListPermissions : IAppMessage
    {
        /// <summary>Si platform admin edita rol de empresa, filtrar por módulos contratados.</summary>
        public int? TenantId { get; set; }
    }
}
