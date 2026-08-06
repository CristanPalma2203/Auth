using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Models
{
    public class ExternalUser : IEntity
    {
        public static string ManualEntryType = "manual";

        public int Id { get; set; }
        /// <summary>Empresa dueña del comprador (Tempora=1, Carbonera=2, …). ERP filtra por esto.</summary>
        public int? TenantId { get; set; }
        public int IdentificationTypeId { get; set; }
        public Catalog IdentificationType { get; set; }
        public string Identifier { get; set; }
        public int PersonTypeId { get; set; }
        public Catalog TipoPersona { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int NationalityId { get; set; }
        public Catalog Nationality { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public int DepartmentId { get; set; }
        public Catalog Department { get; set; }
        public int MunicipalityId { get; set; }
        public Catalog Municipality { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public int? FileId { get; set; }
        public StoredFile StoredFile { get; set; }
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
        public AppUser UserGention { get; set; }
        public string VerificationToken { get; set; }
        public string RejectionReason { get; set; }

        public void RegisterAccount() {
            RegisteredAt = DateTime.Now;
            VerificationToken = Guid.NewGuid().ToString();
            AccessApproved = true;
            EmailVerified = false;
            EmailSent = false;
            EntryType = "WEB";
        }

        public void RequestAccess() {
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
        public void ActulizarImportador(ExternalUser imporN)
        {
            this.MunicipalityId = imporN.MunicipalityId;
            this.DepartmentId = imporN.DepartmentId;
            this.Mobile = imporN.Mobile;
            this.Phone = imporN.Phone;
            this.Email = imporN.Email;
            this.Address =  imporN.Address;
            UpdatedAt = DateTime.Now;
           
        }
  
        public void ManageAccess() {
            AccessApprovedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;

        }
        public void VerifyEmail() {
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
