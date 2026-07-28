using Dominio.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominio.Models
{
    public class Usuario : IEntity
    {
        public static string usuarioInterno = "usuario-interno";
        public static string tipoUsuarioExterno = "usuario-externo";
        public static int idUsuarioAdmin = 1;
        public static string correoUsuarioAdmin = "admin@gmail.com";
        public static List<int> PermisosUsuarioExterno = new List<int>() {25};
        public int Id { get; set; }
        public string Name { get; set; }
        public string AccessIdentifier { get; set; }
        public bool IsActive { get; set; }
        public string Password { get; set; }
        public string TemporaryCode { get; set; }
        public int? DepartamentoId { get; set; }
        public Catalogo Departamento { get; set; }

        public IList<UsuarioRol> Roles { get; set; }
        public bool MustChangePassword { get; set; }

        public void PropietarioCambiaContrasena(string contrasena)
        {
            Password = getPassword(contrasena);
            this.UpdatedAt = DateTime.Now;
            this.MustChangePassword = false;
        }

        public DateTime? RegisteredAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? PasswordResetAt { get; set; }
        public string UserType { get; set; }

        /// <summary>NULL = platform admin; NOT NULL = usuario de una empresa.</summary>
        public int? TenantId { get; set; }
        public Tenant Tenant { get; set; }

        public ICollection<UsuarioRegional> UsuarioRegional { get; set; }
        public ICollection<UsuarioArea> UsuarioArea { get; set; }
        public void RestablecerContrasenaImportador(string contrasena) {
            Password = getPassword(contrasena);
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

        public void Inicializar(string tipoUsuario, IList<int> roles)
        {
            Enable();
            RegisteredAt = DateTime.Now;
            MustChangePassword = true;
            Password = getPassword(Password);
            UserType = tipoUsuario == null ? usuarioInterno : tipoUsuario;
            CrearUsuarioRol(roles);
        }

        public void InicializarExterno(IList<int> roles)
        {
            IsActive = false;
            RegisteredAt = DateTime.Now;
            MustChangePassword = false;
            Password = getPassword(Password);
            UserType = tipoUsuarioExterno;
            CrearUsuarioRol(roles);
        }

        public void ActualizarFecha(IList<int> roles, string tipoUsuario) {
            this.UpdatedAt = DateTime.Now;
            UserType = tipoUsuario == null ? usuarioInterno : tipoUsuario;
            CrearUsuarioRol(roles);
        }

        public static string getPassword(string constrasena)
        {
            return PasswordHelper.getPassword(constrasena);
        }
        public void CrearUsuarioRol(IList<int> permisosLista)
        {
            this.Roles = new List<UsuarioRol>();
            foreach (var item in permisosLista)
            {
                Roles.Add(new UsuarioRol { RolId = item, Usuario = this });
            }

        }

        public void AdministradorCambiaContrasena(string nombre, int? departamento, string contrasena, IList<int> roles, bool activo)
        {
            this.Name = nombre;
            this.IsActive = activo;
            this.DepartamentoId = departamento;
            this.UpdatedAt = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(contrasena))
            {
                Password = getPassword(contrasena);
                MustChangePassword = true;
            };
            this.CrearUsuarioRol(roles);

        }
    }
}
