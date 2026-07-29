using Application.Common;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands
{
    public class GetCatalog: IAppMessage
    {
        public string Type { get; set; }
        public int ParentId { get; set; }
    }
}
 
