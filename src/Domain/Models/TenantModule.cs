using System;

namespace Domain.Models
{
    /// <summary>Módulo contratado / habilitado para un tenant.</summary>
    public class TenantModule : IEntity
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        /// <summary>Código del pack (payments, dte, cms, …).</summary>
        public string ModuleCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
