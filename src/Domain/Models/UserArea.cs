using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class UserArea : IEntity
    {
        public int Id { get; set; }
        public Catalog Area { get; set; }
        public int AreaId { get; set; }
        public AppUser User { get; set; }
        public int UserId { get; set; } 
    }
}
