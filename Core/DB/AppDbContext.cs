using Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.DB
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<CommonDropdown> CommonDropdowns { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<UserVerification> UserVerifications { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<HomePageImage> HomePageImages { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Blog> Blog { get; set; }
        public DbSet<BlogSubscription> BlogSubscription { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<BlogComment> BlogComment { get; set; }
        public DbSet<Dues> Dues { get; set; }
        public DbSet<School> School { get; set; }
        public DbSet<Paystack> Paystack { get; set; }
        public DbSet<DonationPackage> DonationPackage { get; set; }
        public DbSet<Paystack> Paystacks { get; set; }
        public DbSet<SemesterManual> SemesterManual { get; set; }
    }
}
