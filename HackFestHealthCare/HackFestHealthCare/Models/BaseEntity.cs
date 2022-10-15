using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HackFestHealthCare.Models
{
    public class BaseEntity<Guid> : IEntity<Guid>
    {
        public BaseEntity()
        {
            IsActive = true;
            IsDeleted = false;
            CreatedAt = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
        public bool IsTransient()
        {
            return EqualityComparer<Guid>.Default.Equals(Id, default(Guid));
        }

        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
