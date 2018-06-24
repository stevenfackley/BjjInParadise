using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BjjInParadise.Core.Models
{
    [Table("ApplicationUsers")]
    public class User : BaseModel
    {
        [Required]
        [Display(Name = "User Id")]
        public int UserId { get; set; }
        [Required]
        [Display(Name = "Asp.Net User Id")]
        public string AspNetUserId { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Street Address")]
        public string Street { get; set; }
        [Required]
        [Display(Name = "City/Town")]
        public string City { get; set; }
        [Required]
        [Display(Name = "State")]
        public string State { get; set; }
        [Required]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }
        [Required]
        [Display(Name = "Home Gym")]
        public string HomeGym { get; set; }
        [Required]
        [Display(Name = "Country of Residence")]
        public string Country { get; set; }
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

   
    }
}
