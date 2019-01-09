using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BjjInParadise.Core.Models
{
    public class Booking : BaseModel
    {
        public int BookingId { get; set; }
       [Display(Name = "Booking Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime BookingDate { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Amount Paid")]
        public decimal? AmountPaid { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [ForeignKey("Camp")]
        public int CampId { get; set; }

        public virtual Camp Camp { get; set; }

        public int CampRoomOptionId { get; set; }

 

        public string BrainTreeTransactionId { get; set; }
    }
}
