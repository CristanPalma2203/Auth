using Aplicacion.Common;
using Dominio.Service;

namespace Aplicacion.Commands.Usuario
{
    public class ConsultarUsuario: IAppMessage
    {
        public int Id { get; set; }
    }
}
