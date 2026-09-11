using Domain.Helpers;
using Domain.Models;
using Domain.Specifications;
using NUnit.Framework;
using System.Linq;

namespace Auth.Tests
{
    public class FindUserSpecificationsTests
    {
        [Test]
        public void FindUserByIdentifierAndPassword_null_inputs_never_match()
        {
            var spec = new FindUserByIdentifierAndPassword(null, null);
            var users = new[]
            {
                new AppUser { AccessIdentifier = null, Password = null },
                new AppUser { AccessIdentifier = "admin", Password = PasswordHelper.getPassword("secret") }
            };

            Assert.DoesNotThrow(() => spec.Traer());
            Assert.That(users.Where(spec.Traer()), Is.Empty);
        }

        [Test]
        public void FindUserByIdentifierAndPassword_skips_null_identifier_or_hash()
        {
            var hashed = PasswordHelper.getPassword("secret");
            var spec = new FindUserByIdentifierAndPassword("admin", "secret");
            var users = new[]
            {
                new AppUser { AccessIdentifier = null, Password = hashed },
                new AppUser { AccessIdentifier = "admin", Password = null },
                new AppUser { AccessIdentifier = "admin", Password = hashed }
            };

            var matches = users.Where(spec.Traer()).ToList();
            Assert.That(matches, Has.Count.EqualTo(1));
            Assert.That(matches[0].Password, Is.EqualTo(hashed));
        }

        [Test]
        public void FindUserByIdentifierAndPassword_unknown_user_is_empty()
        {
            var spec = new FindUserByIdentifierAndPassword("nobody@example.com", "secret");
            var users = new[]
            {
                new AppUser { AccessIdentifier = "admin", Password = PasswordHelper.getPassword("secret") }
            };

            Assert.That(users.Where(spec.Traer()), Is.Empty);
        }

        [Test]
        public void FindUserByIdentifierAndCode_null_access_identifier_does_not_throw()
        {
            var spec = new FindUserByIdentifierAndCode(null, null);
            var users = new[]
            {
                new AppUser { AccessIdentifier = null, TemporaryCode = null },
                new AppUser { AccessIdentifier = "user@example.com", TemporaryCode = "1234" }
            };

            Assert.DoesNotThrow(() => spec.Traer());
            Assert.That(users.Where(spec.Traer()), Is.Empty);
        }

        [Test]
        public void FindUserByIdentifierAndCode_matches_only_when_identifier_and_code_present()
        {
            var spec = new FindUserByIdentifierAndCode("user@example.com", "1234");
            var users = new[]
            {
                new AppUser { AccessIdentifier = null, TemporaryCode = "1234" },
                new AppUser { AccessIdentifier = "user@example.com", TemporaryCode = null },
                new AppUser { AccessIdentifier = "user@example.com", TemporaryCode = "1234" }
            };

            var matches = users.Where(spec.Traer()).ToList();
            Assert.That(matches, Has.Count.EqualTo(1));
            Assert.That(matches[0].TemporaryCode, Is.EqualTo("1234"));
        }
    }
}
