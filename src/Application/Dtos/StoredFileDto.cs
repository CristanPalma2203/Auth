using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class StoredFileDto 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string PhysicalPath { get; set; }

    }
}
