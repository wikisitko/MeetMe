using Microsoft.AspNetCore.Identity;
using System;

namespace MeetMe.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public IdentityUser UserFrom { get; set; }
        public IdentityUser UserTo { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
