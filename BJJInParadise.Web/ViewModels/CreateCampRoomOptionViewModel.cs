using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BJJInParadise.Web.ViewModels
{
    public class CreateCampRoomOptionViewModel
    {
        public IEnumerable<BjjInParadise.Core.Models.CampRoomOption> CampRoomOptions { get; set; }
        public int CampId { get; set; }
    }
}