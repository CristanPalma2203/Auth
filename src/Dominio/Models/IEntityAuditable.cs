using System;

namespace Dominio.Models
{
    public interface IEntityAuditable: IEntity
    { 
         DateTime CreatedAt { get; set; }
        int CreatedByUserId { get; set; }
        DateTime? UpdatedAt { get; set; }
        int? UpdatedByUserId { get; set; }
    }
}
