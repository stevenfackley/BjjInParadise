using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Web.Mvc;
using BjjInParadise.Business;
using BjjInParadise.Core.Models;

namespace BJJInParadise.Web.ViewModels
{
    public class NewBookingViewModel
    {
        private int _campRoomOptionId;
     
      

        public NewBookingViewModel()
        {

        }

    


        [Required]
        public int UserId { get; set; }

        [Required]
        public int CampId { get; set; }

 
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Room Options")]
        public List<CampRoomOption> RoomOptions { get; set; }

        public int CampRoomOptionId { get; set; }

        public string ClientToken { get; set; }
        public string CampName { get; set; }
        public bool IsLive { get; set; }
        public string PayPalTransactionId { get; set; }

        public decimal AmountPaid { get; set; }
    }

   
}