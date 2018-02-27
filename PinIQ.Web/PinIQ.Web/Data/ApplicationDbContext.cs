using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pinball.Web.Models;

namespace Pinball.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<PinballTable> PinballTable { get; set; }
        public DbSet<Abbreviation> Abbreviation { get; set; }
        public DbSet<PinballTableCatalog> PinballTableCatalog { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<PinballTable>()
                .HasKey(p => p.Id);
            builder.Entity<PinballTable>()
                .HasMany(p => p.Abbreviations)
                .WithOne(a => a.Table)
                .HasForeignKey(a => a.TableID);
            builder.Entity<PinballTable>()
                .Ignore(p => p.DisplayName);

            builder.Entity<PinballTableCatalog>()
                .HasOne(s => s.Uploader)
                .WithMany()
                .HasForeignKey(s => s.UploaderID);
            builder.Entity<PinballTableCatalog>()
                .HasOne(s => s.Publisher)
                .WithMany()
                .HasForeignKey(s => s.PublisherID);
        }
    }
}
