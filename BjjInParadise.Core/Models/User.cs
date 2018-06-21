using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BjjInParadise.Core.Models
{
    public class User : BaseModel
    {
        public int UserId { get; set; }
        public string AspNetUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string HomeGym { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
    }
}
