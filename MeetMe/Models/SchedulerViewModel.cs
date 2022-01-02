using System;
using System.Collections;
using System.Collections.Generic;

namespace MeetMe.Models
{
    public class SchedulerViewModel
    {
        public DateTime Date { get; set; }
        public IEnumerable<MeetingViewModel> Meetings { get; set; }
    }
}
