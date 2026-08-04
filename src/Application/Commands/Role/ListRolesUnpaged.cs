using Application.Common;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Role
{
    public class ListRolesUnpaged : IAppMessage
    {
        public bool all { get; set; }

        /// <summary>
        /// Empresa cuyos roles se quieren listar. Solo lo respeta el admin de plataforma,
        /// que necesita ver los roles de otra empresa al asignarle usuarios.
        /// </summary>
        public int? tenantId { get; set; }
    }
}
