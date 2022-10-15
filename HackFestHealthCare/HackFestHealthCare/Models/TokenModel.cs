using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HackFestHealthCare.Models
{
    public class TokenModel : BaseEntity<Guid>
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string Value { get; set; }

        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public UserProfile User { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime LastModifiedDate { get; set; }
        [Required]
        public DateTime ExpiryTime { get; set; }
    }
}
