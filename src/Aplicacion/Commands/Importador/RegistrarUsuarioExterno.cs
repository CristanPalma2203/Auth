using Aplicacion.Common;

namespace Aplicacion.Commands.Importador
{
    public class RegistrarUsuarioExterno : IAppMessage
    {
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
    }
}
