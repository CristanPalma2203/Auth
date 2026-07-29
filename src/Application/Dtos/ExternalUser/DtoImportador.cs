using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos.ExternalUser
{
   public class ExternalUserDto: IResponse
    {
        public string TipoImportador { get; set; }
        public int IdentificationTypeId { get; set; }
        public int PersonTypeId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int NationalityId { get; set; }
        public CatalogDto Nationality { get; set; }
        public string Identifier { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public int DepartmentId { get; set; }
        public CatalogDto Department { get; set; }
        public int MunicipalityId { get; set; }
        public CatalogDto Municipality { get; set; }
        public CatalogDto IdentificationType { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public int? Id { get; set; }
        public int? IdUsuario { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool EmailSent { get; set; }

        public int? FileId { get; set; }
        public IList<ExternalUserAccessDto> Accesos { get; set; }
        public bool AccessApproved { get; set; }
        public bool EmailVerified { get; set; }
        public DateTime? FechaDenegacionAcceso { get; set; }
        public DateTime? AccessApprovedAt { get; set; }
        public string RejectionReason { get; set; }
  
        public bool UserExist { get; set; }

    }

 
}
