using System;

namespace Dominio.Models
{
    /// <summary>Empresa / cliente SaaS (Tempora, Finca Carbonera, …).</summary>
    public class Tenant : IEntity
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
    }
}
