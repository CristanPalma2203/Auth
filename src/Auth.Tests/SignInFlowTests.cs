using Application.CommandHandlers.AppUser;
using Application.Commands.AppUser;
using Application.Exceptions;
using Application.Services.Validaciones;
using Application.Validators;
using Domain.Helpers;
using Domain.Models;
using Domain.Repositories;
using Domain.Service;
using Domain.Specifications;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Auth.Tests
{
    public class SignInFlowTests
    {
        [Test]
        public void Validator_null_password_is_invalid_credentials_not_500()
        {
            var validator = CreateValidator();

            var ex = Assert.Throws<HttpException>(() =>
                validator.ValidarComando(new SignIn { AppUser = "admin", Password = null }));

            Assert.That(ex.StatusCode, Is.EqualTo(422));
        }

        [Test]
        public void Validator_missing_password_is_invalid_credentials_not_500()
        {
            var validator = CreateValidator();

            var ex = Assert.Throws<HttpException>(() =>
                validator.ValidarComando(new SignIn { AppUser = "admin", Password = "" }));

            Assert.That(ex.StatusCode, Is.EqualTo(422));
        }

        [Test]
        public void Validator_unknown_user_is_invalid_credentials()
        {
            var validator = CreateValidator();

            var ex = Assert.Throws<HttpException>(() =>
                validator.ValidarComando(new SignIn { AppUser = "nobody", Password = "secret" }));

            Assert.That(ex.StatusCode, Is.EqualTo(422));
        }

        [Test]
        public void Validator_valid_shape_and_matching_hash_passes()
        {
            var hashed = PasswordHelper.getPassword("secret");
            var user = new AppUser { AccessIdentifier = "admin", Password = hashed, IsActive = true };
            var validator = CreateValidator(user);

            Assert.DoesNotThrow(() =>
                validator.ValidarComando(new SignIn { AppUser = "admin", Password = "secret" }));
        }

        [Test]
        public void Handler_null_password_is_invalid_credentials_not_500()
        {
            var handler = CreateHandler(null);

            var ex = Assert.Throws<HttpException>(() =>
                handler.Handle(new SignIn { AppUser = "admin", Password = null }));

            Assert.That(ex.StatusCode, Is.EqualTo(422));
        }

        [Test]
        public void Handler_unknown_user_is_invalid_credentials()
        {
            var handler = CreateHandler(null);

            var ex = Assert.Throws<HttpException>(() =>
                handler.Handle(new SignIn { AppUser = "nobody", Password = "secret" }));

            Assert.That(ex.StatusCode, Is.EqualTo(422));
        }

        [Test]
        public void Handler_user_without_password_hash_is_invalid_credentials()
        {
            var handler = CreateHandler(new AppUser { AccessIdentifier = "admin", Password = null });

            var ex = Assert.Throws<HttpException>(() =>
                handler.Handle(new SignIn { AppUser = "admin", Password = "secret" }));

            Assert.That(ex.StatusCode, Is.EqualTo(422));
        }

        private static SignInValidator CreateValidator(AppUser matching = null)
        {
            var repo = new Mock<IAppUserRepository>();
            repo.Setup(r => r.Filter(It.IsAny<Func<AppUser, bool>>()))
                .Returns(Enumerable.Empty<AppUser>());
            repo.Setup(r => r.Filter(It.IsAny<ISpecification<AppUser>>()))
                .Returns<ISpecification<AppUser>>(spec =>
                {
                    if (matching == null)
                    {
                        return Enumerable.Empty<AppUser>();
                    }

                    return new[] { matching }.Where(spec.Traer());
                });

            var auth = new Mock<IAutenticationHelper>();
            return new SignInValidator(repo.Object, auth.Object);
        }

        private static SignInHandler CreateHandler(AppUser user)
        {
            var repo = new Mock<IAppUserRepository>();
            repo.Setup(r => r.GetUserWithRolePermissions(It.IsAny<ISpecification<AppUser>>()))
                .Returns(user);
            return new SignInHandler(
                repo.Object,
                new Mock<IPermissionRepository>().Object,
                new Mock<ITokenService>().Object);
        }
    }
}
