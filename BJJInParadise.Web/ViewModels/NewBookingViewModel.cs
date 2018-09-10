using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Web.Mvc;
using BjjInParadise.Core.Models;

namespace BJJInParadise.Web.ViewModels
{
    public class NewBookingViewModel
    {
  

        public NewBookingViewModel()
        {
          
        }

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
        [Required][MaxLength(12)][MinLength(5)]
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


        [Required]
        public int UserId { get; set; }

        [Required]
        public int CampId { get; set; }

 
        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [DisplayName("Available Camps")]
        public List<SelectListItem> AllAvailableCamps { get; set; }

        [DisplayName("Room Options")]
        public List<SelectListItem> RoomOptions { get; set; }

        public int CampRoomOptionId { get; set; }

        [Required]
        [DisplayName("Credit Card Number")]
        [CreditCard]
        public string CreditCard { get; set; }

        [Required]
        [DisplayName("Expiration Date")]
        [DisplayFormat(DataFormatString = "{0: MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Expiration { get; set; }


        [Required]
        [MaxLength(5)]
        [MinLength(2)]
        public string CVC { get; set; }
        public List<SelectListItem> Countries { get; internal set; }
        public string ClientToken { get; set; }
    }

   
}