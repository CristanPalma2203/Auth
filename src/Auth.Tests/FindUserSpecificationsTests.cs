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
        public void FindUserByIdentifierAndPassword_stg_shape_compares_Password_column()
        {
            // STG qa.smoke Id=5: AccessIdentifier len 23, Password len 64, both non-null. Seed is not empty-hash.
            const string identifier = "qa.smoke@luxware.co";

            var storedPassword = PasswordHelper.getPassword("Secret1!");
            Assert.That(storedPassword, Has.Length.EqualTo(64));

            var user = new AppUser
            {
                Id = 5,
                AccessIdentifier = identifier,
                Password = storedPassword
            };

            var spec = new FindUserByIdentifierAndPassword(identifier, "Secret1!");
            Assert.DoesNotThrow(() => spec.Traer()(user));
            Assert.That(spec.Traer()(user), Is.True);
            Assert.That(user.Password, Is.EqualTo(storedPassword));
        }

        [Test]
        public void FindUserByIdentifierAndPassword_null_request_password_does_not_throw()
        {
            var storedPassword = PasswordHelper.getPassword("Secret1!");
            var user = new AppUser
            {
                AccessIdentifier = "qa.smoke@luxware.co",
                Password = storedPassword
            };

            var spec = new FindUserByIdentifierAndPassword(user.AccessIdentifier, null);
            Assert.DoesNotThrow(() => spec.Traer()(user));
            Assert.That(spec.Traer()(user), Is.False);
        }

        [Test]
        public void FindUserByIdentifierAndPassword_json_plus_address_matches()
        {
            const string stored = "qa.smoke+stg@luxware.co";
            var hashed = PasswordHelper.getPassword("secret");
            var user = new AppUser { AccessIdentifier = stored, Password = hashed };
            var spec = new FindUserByIdentifierAndPassword(stored, "secret");

            Assert.That(spec.Traer()(user), Is.True);
        }

        [Test]
        public void FindUserByIdentifierAndPassword_form_decoded_plus_as_space_still_matches()
        {
            // application/x-www-form-urlencoded: '+' → ' '
            const string stored = "qa.smoke+stg@luxware.co";
            const string incoming = "qa.smoke stg@luxware.co";
            var hashed = PasswordHelper.getPassword("secret");
            var user = new AppUser { AccessIdentifier = stored, Password = hashed };

            Assert.That(Domain.Utilities.RegexUtilities.IsValidEmail(incoming), Is.False,
                "espacio en local-part no es email válido; sin normalizar caía al compare de documento");

            var spec = new FindUserByIdentifierAndPassword(incoming, "secret");
            Assert.That(spec.Traer()(user), Is.True);
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
