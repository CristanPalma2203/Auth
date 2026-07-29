using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class UserRegional : IEntity
    {
        public int Id { get; set; }
        public Catalog Regional { get; set; }
        public int RegionalId { get; set; }
        public AppUser User { get; set; }
        public int UserId { get; set; } 
    }
}
