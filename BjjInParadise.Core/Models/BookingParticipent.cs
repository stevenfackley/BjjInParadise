using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BjjInParadise.Core.Models
{
    public class BookingParticipant : BaseModel
    {
        public int BookingParticipantId { get; set; }
        public int BookingId { get; set; }
        public int ParticipantId { get; set; }

        public virtual Booking Booking { get; set; }
   

    }
}
