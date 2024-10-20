using Domain.Identity;
using Domain.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ApplicationDbContext : IdentityDbContext<EventsAppUser>
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<EventUser> EventUsers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EventUser>()
                .HasKey(eu => new { eu.EventId, eu.UserId });

            modelBuilder.Entity<EventUser>()
                .HasOne(eu => eu.Event)
                .WithMany(e => e.Users)
                .HasForeignKey(eu => eu.EventId);

            modelBuilder.Entity<EventUser>()
                .HasOne(eu => eu.User)
                .WithMany(u => u.Events)
                .HasForeignKey(eu => eu.UserId);

        }
    }
}
