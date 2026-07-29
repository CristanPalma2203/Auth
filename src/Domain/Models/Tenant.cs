using System;

namespace Domain.Models
{
    /// <summary>Empresa / cliente SaaS (Tempora, Finca Carbonera, …).</summary>
    public class Tenant : IEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
