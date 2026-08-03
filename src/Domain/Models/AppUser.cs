using Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Models
{
    public class AppUser : IEntity
    {
        public static string internalUserType = "internal-user";
        public static string externalUserType = "external-user";
        public static int AdminUserId = 1;
        public static string adminUserEmail = "admin@gmail.com";
        public static List<int> ExternalUserPermissionIds = new List<int>() {25};
        public int Id { get; set; }
        public string Name { get; set; }
        public string AccessIdentifier { get; set; }
        public bool IsActive { get; set; }
        public string Password { get; set; }
        public string TemporaryCode { get; set; }
        public int? DepartmentId { get; set; }
        public Catalog Department { get; set; }

        public IList<UserRole> Roles { get; set; }
        public bool MustChangePassword { get; set; }

        public void OwnerChangesPassword(string password)
        {
            Password = getPassword(password);
            this.UpdatedAt = DateTime.Now;
            this.MustChangePassword = false;
        }

        public DateTime? RegisteredAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? PasswordResetAt { get; set; }
        public string UserType { get; set; }

        /// <summary>NULL = platform admin; NOT NULL = Usuario de una empresa.</summary>
        public int? TenantId { get; set; }
        public Tenant Tenant { get; set; }

        /// <summary>Stored file id for profile avatar (Files service).</summary>
        public int? ProfileFileId { get; set; }

        public ICollection<UserRegional> UserRegional { get; set; }
        public ICollection<UserArea> UserArea { get; set; }
        public void ResetExternalUserPassword(string password) {
            Password = getPassword(password);
            PasswordResetAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            MustChangePassword = true;
        }
        public void Enable()
        {
            IsActive = true;
        }
        public void Disable()
        {
            IsActive = false;
        }

        public void Initialize(string tipoUsuario, IList<int> roles)
        {
            Enable();
            RegisteredAt = DateTime.Now;
            MustChangePassword = true;
            Password = getPassword(Password);
            UserType = tipoUsuario == null ? internalUserType : tipoUsuario;
            CreateUserRoles(roles);
        }

        public void InitializeExternal(IList<int> roles)
        {
            IsActive = false;
            RegisteredAt = DateTime.Now;
            MustChangePassword = false;
            Password = getPassword(Password);
            UserType = externalUserType;
            CreateUserRoles(roles);
        }

        public void UpdateProfile(IList<int> roles, string tipoUsuario) {
            this.UpdatedAt = DateTime.Now;
            UserType = tipoUsuario == null ? internalUserType : tipoUsuario;
            CreateUserRoles(roles);
        }

        public static string getPassword(string password)
        {
            return PasswordHelper.getPassword(password);
        }
        public void CreateUserRoles(IList<int> permisosLista)
        {
            this.Roles = new List<UserRole>();
            foreach (var item in permisosLista)
            {
                Roles.Add(new UserRole { RoleId = item, User = this });
            }

        }

        public void AdminChangesPassword(string nombre, int? departamento, string password, IList<int> roles, bool activo)
        {
            this.Name = nombre;
            this.IsActive = activo;
            this.DepartmentId = departamento;
            this.UpdatedAt = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(password))
            {
                Password = getPassword(password);
                MustChangePassword = true;
            };
            this.CreateUserRoles(roles);

        }
    }
}
