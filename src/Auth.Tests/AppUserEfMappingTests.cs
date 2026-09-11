using Domain.Models;
using Domain.Service;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace Auth.Tests
{
    public class AppUserEfMappingTests
    {
        [Test]
        public void AppUser_maps_Password_column_not_PasswordHash()
        {
            using var context = CreateContext();
            var entity = context.Model.FindEntityType(typeof(AppUser));
            Assert.That(entity, Is.Not.Null);
            Assert.That(entity.GetTableName(), Is.EqualTo("app_user"));

            var table = StoreObjectIdentifier.Create(entity, StoreObjectType.Table);
            Assert.That(table.HasValue, Is.True);

            var password = entity.FindProperty(nameof(AppUser.Password));
            Assert.That(password, Is.Not.Null, "AppUser.Password must be mapped");
            Assert.That(password.GetColumnName(table.Value), Is.EqualTo("Password"));

            Assert.That(
                entity.GetProperties().Any(p => p.Name == "PasswordHash" || p.GetColumnName(table.Value) == "PasswordHash"),
                Is.False,
                "STG SQL has no PasswordHash column");

            var access = entity.FindProperty(nameof(AppUser.AccessIdentifier));
            Assert.That(access, Is.Not.Null);
            Assert.That(access.GetColumnName(table.Value), Is.EqualTo("AccessIdentifier"));
        }

        private static AutenticationContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AutenticationContext>()
                .UseSqlServer("Server=127.0.0.1;Database=unused;User ID=x;Password=x;TrustServerCertificate=True")
                .Options;
            return new AutenticationContext(options, Mock.Of<ITokenService>());
        }
    }
}
