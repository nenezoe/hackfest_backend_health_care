using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackFestHealthCare.Models
{
    public interface IEntity<Guid>
    {
        Guid Id { get; set; }

        DateTime CreatedAt { get; set; }
        bool IsTransient();
        bool IsDeleted { get; set; }
        bool IsActive { get; set; }
    }
}
