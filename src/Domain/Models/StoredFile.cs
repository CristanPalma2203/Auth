using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class StoredFile: IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime? RegisteredAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string PhysicalPath { get; set; }
        public string ContentType { get; set; }
        public string Identifier { get; set; }
        public int? UserId { get; set; }
        public AppUser User { get; set; }
    }
}
