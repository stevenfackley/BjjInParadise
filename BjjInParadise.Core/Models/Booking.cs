using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BjjInParadise.Core.Models
{
    public class Booking : BaseModel
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int CampId { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal? AmountPaid { get; set; }

        public virtual User User { get; set; }
        public virtual Camp Camp { get; set; }
    }
}
