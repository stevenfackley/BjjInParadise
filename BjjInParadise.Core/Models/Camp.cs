using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BjjInParadise.Core.Models
{
    public class Camp : BaseModel
    {
        public int CampId { get; set; }
        [Display(Name = "Camp Name")]
        public string CampName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Camp Start Date")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Camp End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<CampRoomOption> CampRoomOptions { get; set; }
        public string HtmlPageText { get; set; }
    }
}
