using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BjjInParadise.Core.Models
{
    public class CampRoomOption : BaseModel
    {
        [Key]
        public int CampRoomOptionId { get; set; }
        [Required]
        public int CampId { get; set; }
        public Camp Camp { get; set; }
        [Required]
        [Display(Name = "Room Type: ")]
        public string RoomType { get; set; }
        [Required]
        [Display(Name = "Cost Per Person: ")]
        [DataType(DataType.Currency)]
        public decimal CostPerPerson { get; set; }
        [Required]
        [Display(Name = "Spots Available: ")]
        public int SpotsAvailable { get; set; }

    }
}
