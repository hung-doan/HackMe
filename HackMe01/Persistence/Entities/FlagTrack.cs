using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HackMe01.Persistence.Entities
{
    public class FlagTrack
    {
        [Key]
        [MaxLength(100)]
        public string Flag { get; set; }
        public string Source { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
