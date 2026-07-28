using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Dtos.Importador
{
   public class DtoImportador: IResponse
    {
        public string TipoImportador { get; set; }
        public int IdentificationTypeId { get; set; }
        public int PersonTypeId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int NacionalidadId { get; set; }
        public DtoCatalogo Nacionalidad { get; set; }
        public string Identificador { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public int DepartamentoId { get; set; }
        public DtoCatalogo Departamento { get; set; }
        public int MunicipioId { get; set; }
        public DtoCatalogo Municipio { get; set; }
        public DtoCatalogo TipoIdentificador { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public int? Id { get; set; }
        public int? IdUsuario { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool EmailSent { get; set; }

        public int? ArchivoId { get; set; }
        public IList<ImportadorAccesosDto> Accesos { get; set; }
        public bool AccessApproved { get; set; }
        public bool EmailVerified { get; set; }
        public DateTime? FechaDenegacionAcceso { get; set; }
        public DateTime? AccessApprovedAt { get; set; }
        public string RejectionReason { get; set; }
  
        public bool UserExist { get; set; }

    }

 
}
