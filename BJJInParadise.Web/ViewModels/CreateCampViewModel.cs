using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BjjInParadise.Core.Models;

namespace BJJInParadise.Web.ViewModels
{
    public class CreateCampViewModel : BaseModel
    {
        [Display(Name = "Camp Name")]
        public string CampName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [AllowHtml]
        [Display(Name = "HtmlPageText")]
        public string HtmlPageText { get; set; }
    }
}