using eMAM.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eMAM.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<User> ApplicationUsers { get; set; }

        public DbSet<Email> Emails { get; set; }

        public DbSet<Status> Statuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            modelBuilder.Entity<User>().HasMany(e => e.OpenedEmails)
                                        .WithOne(x => x.OpenedBy)
                                        .HasForeignKey(g => g.OpenedById);

            modelBuilder.Entity<User>().HasMany(t => t.ClosedEmails)
                                       .WithOne(g => g.ClosedBy)
                                       .HasForeignKey(g => g.ClosedById).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
