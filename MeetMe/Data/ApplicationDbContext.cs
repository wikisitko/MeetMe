using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using MeetMe.Models;

namespace MeetMe.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MeetMe.Models.Meeting> Meeting { get; set; }
        public DbSet<MeetMe.Models.Attendance> Attendance { get; set; }
        public DbSet<MeetMe.Models.Message> Message { get; set; }
    }
}
