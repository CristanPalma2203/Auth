using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Services
{
    /// <summary>
    /// Catálogo de módulos SaaS por tenant.
    /// Cada módulo mapea a un set de permission.Code (menú ERP + API).
    /// </summary>
    public static class TenantModuleCatalog
    {
        public sealed class ModuleDef
        {
            public string Code { get; init; }
            public string Name { get; init; }
            public string Description { get; init; }
            public string Group { get; init; }
            public bool Recommended { get; init; }
            public string[] PermissionCodes { get; init; }
        }

        /// <summary>Administración básica: siempre se incluye al crear rol admin del tenant.</summary>
        public static readonly string[] CoreAdminPermissions =
        {
            "administration",
            "users", "user-list", "user-create", "user-edit", "user-view",
            "roles", "role-list", "role-create", "role-edit", "role-view",
        };

        public static readonly ModuleDef[] All =
        {
            new ModuleDef
            {
                Code = "payments",
                Name = "Pagos (Wompi)",
                Description = "Enlaces, suscripciones y cobros en línea.",
                Group = "Cobros",
                Recommended = true,
                PermissionCodes = new[]
                {
                    "payments", "payment-links", "payment-link-create", "payment-dashboard", "payment-config",
                    "payment-subscriptions", "payment-subscription-create",
                    "payment-customers", "payment-customer-create", "payment-customer-view",
                    "payment-products", "payment-product-create",
                    "external-users", "external-user-list", "external-user-view", "external-user-edit",
                    "manage-external-user",
                },
            },
            new ModuleDef
            {
                Code = "dte",
                Name = "Facturación electrónica",
                Description = "Emisión y configuración de DTE.",
                Group = "Cobros",
                Recommended = false,
                PermissionCodes = new[]
                {
                    "operations",
                    "dte", "dte-list", "dte-view", "dte-emit", "dte-config", "dte-dashboard",
                },
            },
            new ModuleDef
            {
                Code = "email",
                Name = "Correos",
                Description = "Plantillas y marca del remitente.",
                Group = "Comunicación",
                Recommended = true,
                PermissionCodes = new[] { "email-brand" },
            },
            new ModuleDef
            {
                Code = "carts",
                Name = "Carritos abandonados",
                Description = "Carritos y recordatorios por correo.",
                Group = "Comunicación",
                Recommended = true,
                PermissionCodes = new[] { "carts" },
            },
            new ModuleDef
            {
                Code = "cms",
                Name = "Contenido de tienda",
                Description = "Páginas y contenido de la tienda.",
                Group = "Tienda",
                Recommended = true,
                PermissionCodes = new[]
                {
                    "marketing", "storefront-cms", "storefront-cms-edit",
                },
            },
            new ModuleDef
            {
                Code = "cms-products",
                Name = "Catálogo de productos",
                Description = "Productos para tienda y cobro.",
                Group = "Tienda",
                Recommended = false,
                PermissionCodes = new[]
                {
                    "marketing",
                    "storefront-products", "storefront-products-edit",
                    "payment-products", "payment-product-create",
                },
            },
            new ModuleDef
            {
                Code = "cms-publications",
                Name = "Publicaciones",
                Description = "Artículos y blog de la tienda.",
                Group = "Tienda",
                Recommended = false,
                PermissionCodes = new[]
                {
                    "marketing",
                    "storefront-publications", "storefront-publications-edit",
                },
            },
            new ModuleDef
            {
                Code = "sales-orders",
                Name = "Pedidos",
                Description = "Pedidos y conversión a venta o compra.",
                Group = "Operaciones",
                Recommended = true,
                PermissionCodes = new[]
                {
                    "operations",
                    "sales-orders", "sales-order-list", "sales-order-create",
                    "sales-order-edit", "sales-order-view", "sales-order-confirm", "sales-order-generate",
                },
            },
            new ModuleDef
            {
                Code = "sales",
                Name = "Ventas",
                Description = "Documentos de venta.",
                Group = "Operaciones",
                Recommended = false,
                PermissionCodes = new[]
                {
                    "operations",
                    "sales", "sale-list", "sale-view", "sale-edit",
                },
            },
            new ModuleDef
            {
                Code = "meta-crm",
                Name = "Meta CRM",
                Description = "Inbox de Instagram, Messenger y WhatsApp.",
                Group = "Marketing",
                Recommended = false,
                PermissionCodes = new[]
                {
                    "marketing", "meta-crm", "meta-crm-config",
                },
            },
            new ModuleDef
            {
                Code = "reporting",
                Name = "Reportería",
                Description = "Consulta de pedidos y ventas.",
                Group = "Operaciones",
                Recommended = false,
                PermissionCodes = new[]
                {
                    "operations", "erp-reporting",
                },
            },
        };

        public static IReadOnlyList<string> DefaultCodesForNewTenant() =>
            All.Where(m => m.Recommended).Select(m => m.Code).ToList();

        public static HashSet<string> NormalizeCodes(IEnumerable<string> codes)
        {
            var valid = new HashSet<string>(All.Select(m => m.Code), StringComparer.OrdinalIgnoreCase);
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (codes == null) return result;
            foreach (var raw in codes)
            {
                var c = (raw ?? "").Trim().ToLowerInvariant();
                if (valid.Contains(c)) result.Add(c);
            }
            return result;
        }

        public static HashSet<string> PermissionCodesForModules(IEnumerable<string> moduleCodes)
        {
            var selected = NormalizeCodes(moduleCodes);
            var perms = new HashSet<string>(CoreAdminPermissions, StringComparer.OrdinalIgnoreCase);
            foreach (var mod in All)
            {
                if (!selected.Contains(mod.Code)) continue;
                foreach (var p in mod.PermissionCodes)
                    perms.Add(p);
            }
            return perms;
        }

        public static object CatalogPayload() =>
            All.Select(m => new
            {
                code = m.Code,
                name = m.Name,
                description = m.Description,
                group = m.Group,
                recommended = m.Recommended,
            }).ToList();
    }
}
