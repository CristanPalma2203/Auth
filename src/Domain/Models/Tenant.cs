using System;

namespace Domain.Models
{
    /// <summary>Empresa / cliente SaaS (Tempora, Finca Carbonera, …).</summary>
    public class Tenant : IEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        /// <summary>Identidad fiscal del tenant (receptor DTE).</summary>
        public string Nit { get; set; }
        public string Nrc { get; set; }
        public string RazonSocial { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        /// <summary>Catálogo 'tipo-comercio'.</summary>
        public int? BusinessTypeId { get; set; }

        /// <summary>Mensaje que ve el cliente antes de pagar.</summary>
        public string CheckoutMessage { get; set; }

        /// <summary>Nombre visible en correos (fallback: Name).</summary>
        public string BrandName { get; set; }
        public string BrandPrimaryColor { get; set; }
        public string BrandBgColor { get; set; }
        public string BrandInkColor { get; set; }
        public string BrandLogoUrl { get; set; }
        /// <summary>Base pública de la tienda (verify + links).</summary>
        public string StorefrontPublicUrl { get; set; }
        public string EmailFromDisplay { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
