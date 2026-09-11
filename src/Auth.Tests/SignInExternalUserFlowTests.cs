using Application.CommandHandlers.AppUser;
using Application.CommandHandlers.ExternalUser;
using Application.Commands.AppUser;
using Application.Commands.ExternalUser;
using Application.Dtos.AppUser;
using Application.Exceptions;
using Application.Services.Validaciones;
using Application.Validators.ExternalUser;
using Domain.Helpers;
using Domain.Models;
using Domain.Repositories;
using Domain.Service;
using Domain.Specifications;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Auth.Tests
{
    public class SignInExternalUserFlowTests
    {
        [Test]
        public void Storefront_rejects_internal_user_with_clear_403_not_incorrect_credentials()
        {
            var hashed = PasswordHelper.getPassword("secret");
            var internalUser = new AppUser
            {
                Id = 5,
                AccessIdentifier = "qa.smoke+stg@luxware.co",
                Password = hashed,
                IsActive = true,
                UserType = AppUser.internalUserType,
                TenantId = 1,
                Roles = new List<UserRole>()
            };

            var validator = CreateValidator(internalUser);
            Assert.DoesNotThrow(() => validator.ValidarComando(new SignInExternalUser
            {
                AppUser = internalUser.AccessIdentifier,
                Password = "secret"
            }));

            var handler = CreateHandler(internalUser, profile: null);
            var ex = Assert.Throws<HttpException>(() => handler.Handle(new SignInExternalUser
            {
                AppUser = internalUser.AccessIdentifier,
                Password = "secret"
            }));

            Assert.That(ex.StatusCode, Is.EqualTo(403));
            Assert.That(ex.Message, Is.EqualTo(StorefrontLoginMessages.InternalUserNotAllowed));
            Assert.That(ex.Message, Does.Not.Contain("incorrecto"));
        }

        [Test]
        public void Storefront_rejects_external_user_without_external_user_row()
        {
            var user = ExternalCustomer("buyer@luxware.co", "secret");
            var handler = CreateHandler(user, profile: null);

            var ex = Assert.Throws<HttpException>(() => handler.Handle(new SignInExternalUser
            {
                AppUser = user.AccessIdentifier,
                Password = "secret"
            }));

            Assert.That(ex.StatusCode, Is.EqualTo(403));
            Assert.That(ex.Message, Is.EqualTo(StorefrontLoginMessages.InternalUserNotAllowed));
        }

        [Test]
        public void Storefront_rejects_unverified_email()
        {
            var user = ExternalCustomer("buyer@luxware.co", "secret");
            var profile = new ExternalUser
            {
                Email = user.AccessIdentifier,
                Identifier = user.AccessIdentifier,
                EmailVerified = false,
                AccessApproved = true
            };
            var handler = CreateHandler(user, profile);

            var ex = Assert.Throws<HttpException>(() => handler.Handle(new SignInExternalUser
            {
                AppUser = user.AccessIdentifier,
                Password = "secret"
            }));

            Assert.That(ex.StatusCode, Is.EqualTo(422));
            Assert.That(ex.Message, Is.EqualTo(StorefrontLoginMessages.EmailNotVerified));
        }

        [Test]
        public void Storefront_rejects_unapproved_access()
        {
            var user = ExternalCustomer("buyer@luxware.co", "secret");
            var profile = new ExternalUser
            {
                Email = user.AccessIdentifier,
                Identifier = user.AccessIdentifier,
                EmailVerified = true,
                AccessApproved = false
            };
            var handler = CreateHandler(user, profile);

            var ex = Assert.Throws<HttpException>(() => handler.Handle(new SignInExternalUser
            {
                AppUser = user.AccessIdentifier,
                Password = "secret"
            }));

            Assert.That(ex.StatusCode, Is.EqualTo(422));
            Assert.That(ex.Message, Is.EqualTo(StorefrontLoginMessages.AccessNotApproved));
        }

        [Test]
        public void Storefront_accepts_external_user_with_verified_approved_profile_without_roles()
        {
            var user = ExternalCustomer("buyer@luxware.co", "secret");
            var profile = new ExternalUser
            {
                Email = user.AccessIdentifier,
                Identifier = user.AccessIdentifier,
                EmailVerified = true,
                AccessApproved = true
            };
            var handler = CreateHandler(user, profile);

            var response = handler.Handle(new SignInExternalUser
            {
                AppUser = user.AccessIdentifier,
                Password = "secret"
            });

            var login = response as UserLoginDto;
            Assert.That(login, Is.Not.Null);
            Assert.That(login.Token, Is.EqualTo("storefront-token"));
            Assert.That(login.UserType, Is.EqualTo(AppUser.externalUserType));
            Assert.That(login.AccessIdentifier, Is.EqualTo(user.AccessIdentifier));
        }

        [Test]
        public void Storefront_accepts_email_alias_in_json_body()
        {
            var user = ExternalCustomer("buyer@luxware.co", "secret");
            var profile = new ExternalUser
            {
                Email = user.AccessIdentifier,
                Identifier = user.AccessIdentifier,
                EmailVerified = true,
                AccessApproved = true
            };
            var handler = CreateHandler(user, profile);

            var response = handler.Handle(new SignInExternalUser
            {
                Email = user.AccessIdentifier,
                Password = "secret"
            });

            Assert.That(response, Is.InstanceOf<UserLoginDto>());
        }

        [Test]
        public void Storefront_wrong_password_is_422_incorrect_credentials()
        {
            var validator = CreateValidator(matching: null);
            var ex = Assert.Throws<HttpException>(() => validator.ValidarComando(new SignInExternalUser
            {
                AppUser = "buyer@luxware.co",
                Password = "nope"
            }));

            Assert.That(ex.StatusCode, Is.EqualTo(422));
            Assert.That(ex.Message, Does.Contain("incorrecto"));
        }

        [Test]
        public void Erp_AppUser_login_still_allows_internal_user()
        {
            var hashed = PasswordHelper.getPassword("secret");
            var internalUser = new AppUser
            {
                Id = 5,
                AccessIdentifier = "qa.smoke+stg@luxware.co",
                Password = hashed,
                IsActive = true,
                UserType = AppUser.internalUserType,
                TenantId = 1,
                Roles = new List<UserRole>()
            };

            var tokens = new Mock<ITokenService>();
            tokens.Setup(t => t.CreateOrGetToken(It.IsAny<AppUser>())).Returns("erp-token");
            var repo = new Mock<IAppUserRepository>();
            repo.Setup(r => r.GetUserWithRolePermissions(It.IsAny<ISpecification<AppUser>>()))
                .Returns(internalUser);

            var handler = new SignInHandler(
                repo.Object,
                new Mock<IPermissionRepository>().Object,
                tokens.Object);

            var response = handler.Handle(new SignIn
            {
                AppUser = internalUser.AccessIdentifier,
                Password = "secret"
            });

            var login = response as UserLoginDto;
            Assert.That(login, Is.Not.Null);
            Assert.That(login.Token, Is.EqualTo("erp-token"));
            Assert.That(login.UserType, Is.EqualTo(AppUser.internalUserType));
        }

        private static AppUser ExternalCustomer(string identifier, string password)
        {
            return new AppUser
            {
                Id = 20,
                AccessIdentifier = identifier,
                Password = PasswordHelper.getPassword(password),
                IsActive = true,
                UserType = AppUser.externalUserType,
                TenantId = 1,
                Roles = new List<UserRole>()
            };
        }

        private static SignInExternalUserValidator CreateValidator(AppUser matching)
        {
            var repo = new Mock<IAppUserRepository>();
            repo.Setup(r => r.Filter(It.IsAny<System.Func<AppUser, bool>>()))
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

            return new SignInExternalUserValidator(repo.Object, new Mock<IAutenticationHelper>().Object);
        }

        private static SignInExternalUserHandler CreateHandler(AppUser user, ExternalUser profile)
        {
            var users = new Mock<IAppUserRepository>();
            users.Setup(r => r.GetUserWithRolePermissions(It.IsAny<ISpecification<AppUser>>()))
                .Returns(user);

            var externals = new Mock<IExternalUserRepository>();
            externals.Setup(r => r.Filter(It.IsAny<ISpecification<ExternalUser>>()))
                .Returns<ISpecification<ExternalUser>>(spec =>
                {
                    if (profile == null)
                    {
                        return Enumerable.Empty<ExternalUser>();
                    }

                    return new[] { profile }.Where(spec.Traer());
                });

            var tokens = new Mock<ITokenService>();
            tokens.Setup(t => t.CreateOrGetToken(It.IsAny<AppUser>())).Returns("storefront-token");

            return new SignInExternalUserHandler(
                users.Object,
                externals.Object,
                new Mock<IPermissionRepository>().Object,
                tokens.Object);
        }
    }
}
