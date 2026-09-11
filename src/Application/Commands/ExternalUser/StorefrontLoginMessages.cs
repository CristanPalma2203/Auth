namespace Application.Commands.ExternalUser
{
    /// <summary>
    /// Mensajes de POST /api/ExternalUser/login. Distintos de
    /// "AppUser o contraseña es incorrecto" para que QA/FE no los confundan.
    /// </summary>
    public static class StorefrontLoginMessages
    {
        public const string InvalidCredentials = "AppUser o contraseña es incorrecto";
        public const string InactiveUser = "AppUser Inactivo";
        public const string InternalUserNotAllowed = "Este usuario no puede iniciar sesión en la tienda";
        public const string EmailNotVerified = "Debe verificar su correo antes de iniciar sesión";
        public const string AccessNotApproved = "Su acceso a la tienda aún no ha sido aprobado";
    }
}
