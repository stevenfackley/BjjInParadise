using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BjjInParadise.Core.Models;

namespace BJJInParadise.Web.ViewModels
{
    public class NewBookingViewModel
    {
        public NewBookingViewModel()
        {
            AdditionalParticipants = new List<Participant>();
        }
        [Required]
        public User User { get; set; }


        [Required]
        public int UserId { get; set; }

        [Required]
        public int CampId { get; set; }

        public List<Participant> AdditionalParticipants { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [DisplayName("Available Camps")]
        public List<SelectListItem> AllAvailableCamps { get; set; }

        [DisplayName("Room Options")]
        public List<SelectListItem> RoomOptions { get; set; }

        public int CampRoomOptionId { get; set; }
    }
}