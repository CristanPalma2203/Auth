using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Rules
{
    public interface ICambioPassword : IRule
    {
        IReglaRespuesta verificarPasswor(int idUsuario, string passwor);
    }

    public interface IReglaRespuesta { 
        bool Cumple { get; set; }
    }

    public class RegalRespuestaBasica : IReglaRespuesta
    {
        public bool Cumple { get; set; }
    }
}
