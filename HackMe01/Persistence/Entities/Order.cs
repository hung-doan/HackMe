using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HackMe01.Persistence.Entities
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public string Note { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
