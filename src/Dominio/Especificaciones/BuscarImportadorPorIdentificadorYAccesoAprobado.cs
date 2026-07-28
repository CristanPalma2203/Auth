using Dominio.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Especificaciones
{
    public class BuscarImportadorPorIdentificadorYAccesoAprobado : ISpecification<UsuarioExterno>
    { 
    private readonly string identificador;

    public BuscarImportadorPorIdentificadorYAccesoAprobado(string identificador)
    {
        this.identificador = identificador;
    }

    public Func<UsuarioExterno, bool> Traer()
    {
        return new Func<UsuarioExterno, bool>(c => c.AccessApproved==true && c.Identificador.Replace("-", "").Trim() == identificador.Replace("-", "").Trim());

    }
}
}