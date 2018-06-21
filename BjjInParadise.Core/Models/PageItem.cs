using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BjjInParadise.Core.Models
{
    public class PageItem : BaseModel
    {
        public int PageItemId { get; set; }
        public int PageId { get; set; }
        public string PageItemDesc { get; set; }
        public string Content { get; set; }
    }
}
