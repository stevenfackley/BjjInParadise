using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Amount Paid")]
        public decimal? AmountPaid { get; set; }

        public virtual User User { get; set; }
        public virtual Camp Camp { get; set; }
    }
}
