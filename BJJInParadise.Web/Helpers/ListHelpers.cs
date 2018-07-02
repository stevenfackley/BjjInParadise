using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BJJInParadise.Web.Helpers
{
    public static class ListHelpers
    {
        public static List<SelectListItem> AddEmpty(this IEnumerable<SelectListItem> list)
        {
            var lists = new List<SelectListItem> { new SelectListItem { Text = "-- Select --", Value = "0" } };
            lists.AddRange(list);
            return lists;
        }
    }
}