using Application.Common;
using Domain.Helpers;

namespace Application.Commands.ExternalUser
{
    /// <summary>
    /// Login de storefront (Tempora / compradores). Mismo body que AppUser/login:
    /// JSON { "appUser", "password" }. También acepta { "email", "password" }.
    /// </summary>
    public class SignInExternalUser : IAppMessage
    {
        public string AppUser { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string Identifier =>
            AccessIdentifierNormalizer.Normalize(
                !string.IsNullOrWhiteSpace(AppUser) ? AppUser : Email);
    }
}
