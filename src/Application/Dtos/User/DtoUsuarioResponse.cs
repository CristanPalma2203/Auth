using System.Collections.Generic;

namespace Application.Dtos.AppUser
{
    public class DtoUsuarioResponse : UserBaseDto, IResponse
    {
        public string DepartamentoDescripcion { get; set; }
    }

}
