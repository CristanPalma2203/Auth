using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominio.Models
{
    public class UsuarioExterno : IEntity
    {
        public static string TipoIngresoManual = "manual";

        public int Id { get; set; }
        public int IdentificationTypeId { get; set; }
        public Catalogo TipoIdentificador { get; set; }
        public string Identificador { get; set; }
        public int PersonTypeId { get; set; }
        public Catalogo TipoPersona { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int NacionalidadId { get; set; }
        public Catalogo Nacionalidad { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public int DepartamentoId { get; set; }
        public Catalogo Departamento { get; set; }
        public int MunicipioId { get; set; }
        public Catalogo Municipio { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public int? ArchivoId { get; set; }
        public Archivo Archivo { get; set; }
        public string EntryType { get; set; }
        public DateTime? RegisteredAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool EmailSent { get; set; }
        public DateTime? EmailSentAt { get; set; }
        public bool EmailVerified { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public bool AccessApproved { get; set; }
        public DateTime? AccessApprovedAt { get; set; }
        public int? ManagedByUserId { get; set; }
        public Usuario UsuarioGention { get; set; }
        public string VerificationToken { get; set; }
        public string RejectionReason { get; set; }

        public void RegistrarCuenta() {
            RegisteredAt = DateTime.Now;
            VerificationToken = Guid.NewGuid().ToString();
            AccessApproved = true;
            EmailVerified = false;
            EmailSent = false;
            EntryType = "WEB";
        }

        public void SolicitarAcceso() {
            RegisteredAt = DateTime.Now;
            VerificationToken = Guid.NewGuid().ToString();
            AccessApproved = false;
            EmailVerified = false;
            EntryType = "WEB";
        }

        public void AmpliarAccesos(List<TipoProductoResponse> listaNueva, List<TipoProductoResponse> listaVieja)
        {
            EntryType = "WEB";
            EmailVerified = false;
            AccessApprovedAt = null;
            foreach (var productosV in listaVieja)
            {
                foreach (var productosN in listaNueva)
                {
                    if (productosV.Id == productosN.Id && productosV.IsChecked == true && productosN.IsChecked == false) {
                        productosN.IsChecked = true;
                    }
                } 
            }


            EmailSentAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        public void DenegarAcceso(int usuarioDeniega, string motivoRechazo) {
            ManagedByUserId = usuarioDeniega;
            UpdatedAt = DateTime.Now;
            AccessApproved = false;
            RejectionReason = motivoRechazo;
        }
        public void ActulizarImportador(UsuarioExterno imporN)
        {
            this.MunicipioId = imporN.MunicipioId;
            this.DepartamentoId = imporN.DepartamentoId;
            this.Mobile = imporN.Mobile;
            this.Phone = imporN.Phone;
            this.Email = imporN.Email;
            this.Address =  imporN.Address;
            UpdatedAt = DateTime.Now;
           
        }
  
        public void GestionarAcceso() {
            AccessApprovedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;

        }
        public void VerificarCorreo() {
            UpdatedAt = DateTime.Now;
            EmailVerified = true;
            EmailVerifiedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        public void FinalizarEnvitacion(int usuarioApruebaAcceso, List<int> accesos)
        {
            EmailSent = true;
            AccessApprovedAt = DateTime.Now;
            AccessApproved = true;
            EmailSentAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            ManagedByUserId = usuarioApruebaAcceso;
        }

       
    }
}
