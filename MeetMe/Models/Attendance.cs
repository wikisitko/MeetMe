using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetMe.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public Meeting Meeting { get; set; }
        public IdentityUser Attendee { get; set; }
        public bool Confirmation { get; set; }
    }
}
