using Application.Common;

namespace Application.Commands.ExternalUser
{
    public class RegisterExternalUser : IAppMessage
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        /// <summary>storefront | erp — define URL de verificacion de correo</summary>
        public string Origen { get; set; }
    }
}
