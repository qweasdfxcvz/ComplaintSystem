using ComplaintSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ComplaintSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ComplaintType> ComplaintTypes { get; set; }
        public DbSet<ComplaintStatus> ComplaintStatuses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<MessageReply> MessageReplies { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // FromUser (الذي أنشأ الإحالة)
            builder.Entity<Assignment>()
                .HasOne(a => a.FromUser)
                .WithMany(u => u.FromAssignments)
                .HasForeignKey(a => a.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ToUser (المُحال إليه)
            builder.Entity<Assignment>()
                .HasOne(a => a.ToUser)
                .WithMany(u => u.ToAssignments)
                .HasForeignKey(a => a.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // العلاقة مع Complaint
            builder.Entity<Assignment>()
                .HasOne(a => a.Complaint)
                .WithMany(c => c.Assignments)
                .HasForeignKey(a => a.ComplaintId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MessageReply>()
                .HasOne(m => m.Complaint)
                .WithMany(c => c.MessageReplies)
                .HasForeignKey(m => m.ComplaintId)
                .OnDelete(DeleteBehavior.Restrict);


        }

    }
}
