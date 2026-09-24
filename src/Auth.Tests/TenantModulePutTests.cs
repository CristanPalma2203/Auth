using Application.Services.Validaciones;
using Domain.Helpers;
using Domain.Models;
using Domain.Service;
using Domain.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Controllers;

namespace Auth.Tests
{
    /// <summary>
    /// Athenea Fix A: en PUT Tenant, Modules es el set cuando viene.
    /// ModuleAssignments solo superpone tier/settings (o el set si Modules es null).
    /// </summary>
    public class TenantModulePutTests
    {
        [Test]
        public void Put_modules_win_over_stale_assignments_and_persist_pedidos_and_ventas()
        {
            using var db = CreateContext();
            var tenant = SeedTenant(db, "carts", "email");
            var controller = CreatePlatformAdminController(db);

            controller.Put(tenant.Id, new TenantController.TenantBody
            {
                Modules = new List<string> { "carts", "email", "sales-orders", "sales" },
                ModuleAssignments = new List<TenantController.ModuleAssignmentBody>
                {
                    new TenantController.ModuleAssignmentBody
                    {
                        Code = "carts",
                        TierCode = "growth",
                        SettingsJson = "{\"from\":\"assignment\"}"
                    },
                    new TenantController.ModuleAssignmentBody { Code = "email" },
                }
            });

            var rows = db.TenantModules.AsNoTracking().Where(m => m.TenantId == tenant.Id).ToList();
            var codes = rows.Select(m => m.ModuleCode).ToList();
            Assert.That(codes, Does.Contain("sales-orders"));
            Assert.That(codes, Does.Contain("sales"));
            Assert.That(codes, Is.EquivalentTo(new[] { "carts", "email", "sales-orders", "sales" }));

            var carts = rows.Single(m => m.ModuleCode == "carts");
            Assert.That(carts.TierCode, Is.EqualTo("growth"));
            Assert.That(carts.SettingsJson, Is.EqualTo("{\"from\":\"assignment\"}"));
        }

        [Test]
        public void Put_null_modules_uses_assignment_codes_as_the_set()
        {
            using var db = CreateContext();
            var tenant = SeedTenant(db, "cms", "email");
            var controller = CreatePlatformAdminController(db);

            controller.Put(tenant.Id, new TenantController.TenantBody
            {
                Modules = null,
                ModuleAssignments = new List<TenantController.ModuleAssignmentBody>
                {
                    new TenantController.ModuleAssignmentBody { Code = "carts", TierCode = "base" },
                    new TenantController.ModuleAssignmentBody { Code = "sales-orders" },
                }
            });

            var rows = db.TenantModules.AsNoTracking().Where(m => m.TenantId == tenant.Id).ToList();
            Assert.That(rows.Select(m => m.ModuleCode), Is.EquivalentTo(new[] { "carts", "sales-orders" }));
            Assert.That(rows.Single(m => m.ModuleCode == "carts").TierCode, Is.EqualTo("base"));
        }

        [Test]
        public void Put_empty_modules_clears_tenant_modules_even_when_assignments_are_stale()
        {
            using var db = CreateContext();
            var tenant = SeedTenant(db, "carts", "email", "sales");
            var controller = CreatePlatformAdminController(db);

            controller.Put(tenant.Id, new TenantController.TenantBody
            {
                Modules = new List<string>(),
                ModuleAssignments = new List<TenantController.ModuleAssignmentBody>
                {
                    new TenantController.ModuleAssignmentBody { Code = "carts" },
                    new TenantController.ModuleAssignmentBody { Code = "email" },
                }
            });

            var codes = db.TenantModules.AsNoTracking()
                .Where(m => m.TenantId == tenant.Id)
                .Select(m => m.ModuleCode)
                .ToList();
            Assert.That(codes, Is.Empty);
        }

        [Test]
        public void NormalizeCodes_keeps_sales_orders_and_sales()
        {
            var codes = TenantModuleCatalog.NormalizeCodes(new[] { "sales-orders", "sales", "no-such-module" });

            Assert.That(codes, Does.Contain("sales-orders"));
            Assert.That(codes, Does.Contain("sales"));
            Assert.That(codes, Does.Not.Contain("no-such-module"));
        }

        private static Tenant SeedTenant(AutenticationContext db, params string[] moduleCodes)
        {
            var tenant = new Tenant
            {
                Code = "tempora",
                Name = "Tempora",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Tenants.Add(tenant);
            db.SaveChanges();

            foreach (var code in moduleCodes)
            {
                db.TenantModules.Add(new TenantModule
                {
                    TenantId = tenant.Id,
                    ModuleCode = code,
                    CreatedAt = DateTime.UtcNow
                });
            }

            db.SaveChanges();
            return tenant;
        }

        private static TenantController CreatePlatformAdminController(AutenticationContext db)
        {
            var tenantContext = new Mock<ITenantContext>();
            tenantContext.SetupGet(t => t.IsPlatformAdmin).Returns(true);
            tenantContext.Setup(t => t.EnsureSameTenantOrPlatform(It.IsAny<int?>()));

            return new TenantController(
                db,
                new Mock<IAutenticationHelper>().Object,
                tenantContext.Object,
                Mock.Of<IEmailHelper>());
        }

        private static AutenticationContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AutenticationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var db = new AutenticationContext(options, Mock.Of<ITokenService>());
            db.Database.EnsureCreated();
            return db;
        }
    }
}
