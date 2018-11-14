using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HackMe01.Persistence.Entities
{
    public class DebitAccount
    {
        [Key]
        [MaxLength(100)]
        public string UserId { get; set; }
        public decimal Balance { get; set; }

    }
}
