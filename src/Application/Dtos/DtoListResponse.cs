using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class DtoListResponse<T>:IResponse
    {
        public IList<T> Lista { get; set; }
    }

}
