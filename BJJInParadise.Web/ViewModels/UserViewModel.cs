using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BjjInParadise.Core.Models;

namespace BJJInParadise.Web.ViewModels
{
    public class UserViewModel : BaseModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string AspNetUserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public string HomeGym { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        public bool IsAdmin { get; set; }
    }

    public class UserBookingViewModel : UserViewModel
    {

        public IEnumerable<Booking> BookedCamps { get; set; }
    }
}