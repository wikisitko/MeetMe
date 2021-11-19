using System;

namespace MeetMe.Models
{
    public class MeetingViewModel
    {
        public Meeting Meeting { get; set; }
        public bool IsAuthor { get; set; }
        public bool Confirmed { get; set; }
    }
}
