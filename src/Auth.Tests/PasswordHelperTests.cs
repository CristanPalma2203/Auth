using Domain.Helpers;
using NUnit.Framework;
using System;

namespace Auth.Tests
{
    public class PasswordHelperTests
    {
        [Test]
        public void GetHash_null_password_does_not_throw()
        {
            byte[] hash = null;
            Assert.DoesNotThrow(() => hash = PasswordHelper.GetHash(null));
            Assert.That(hash, Is.Not.Null);
            Assert.That(hash, Is.Empty);
        }

        [Test]
        public void GetHash_empty_password_does_not_throw()
        {
            byte[] hash = null;
            Assert.DoesNotThrow(() => hash = PasswordHelper.GetHash(string.Empty));
            Assert.That(hash, Is.Not.Null);
            Assert.That(hash, Is.Empty);
        }

        [Test]
        public void getPassword_null_or_empty_is_invalid_credentials_not_exception()
        {
            Assert.That(PasswordHelper.getPassword(null), Is.Null);
            Assert.That(PasswordHelper.getPassword(string.Empty), Is.Null);
        }

        [Test]
        public void GetHash_valid_password_returns_sha256()
        {
            var hash = PasswordHelper.GetHash("secret");
            Assert.That(hash, Has.Length.EqualTo(32));
            Assert.That(PasswordHelper.getPassword("secret"), Is.Not.Null.And.Not.Empty);
        }
    }
}
