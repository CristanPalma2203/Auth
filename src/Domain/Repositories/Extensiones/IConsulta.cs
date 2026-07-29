using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Repositories.Extenciones
{
    public interface IConsulta
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }
    }
}
