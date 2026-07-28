using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Models
{
    public class Catalogo : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Abbreviation { get; set; }
        public int? ParentId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedByUserId { get; set; }

    }
}
