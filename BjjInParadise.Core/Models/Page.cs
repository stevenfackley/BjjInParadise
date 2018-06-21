using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BjjInParadise.Core.Models
{
    public class Page : BaseModel
    {
        public int PageId { get; set; }
        public string PageName { get; set; }

        public virtual ICollection<PageItem> PageItems { get; set; }
    }
}
