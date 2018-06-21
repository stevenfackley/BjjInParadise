using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BjjInParadise.Core.Models;

namespace BJJInParadise.Web.ViewModels
{
    public class NewBookingViewModel
    {
        public NewBookingViewModel()
        {
            AdditionalParticipants = new List<Participant>();
            AvailableCamps = new List<Camp>();
        }
        [Required]
        public User User { get; set; }


        [Required]
        public int UserId { get; set; }

        [Required]
        public int CampId { get; set; }

        public Camp SelectedCamp { get; set; }

        public List<Camp> AvailableCamps { get; set; }

        public List<Participant> AdditionalParticipants { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}