using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos.AppUser
{
    public class UserDto : UserBaseDto
    {
        public string Password { get; set; }
        /// <summary>Debe coincidir con Password cuando se envía una contraseña manual.</summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Marca explícita para crear un usuario sin empresa (admin de plataforma).
        /// Evita que un TenantId olvidado convierta al usuario en admin de plataforma por accidente.
        /// </summary>
        public bool IsPlatformUser { get; set; }
    }

}
