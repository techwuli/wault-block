using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WaultBlock.Models;

namespace WaultBlock.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<WaultWallet> WaultWallets { get; set; }

        public virtual DbSet<WaultWalletRecord> WaultWalletRecords { get; set; }

        public virtual DbSet<ClaimDefinition> ClaimDefinitions { get; set; }

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

            builder.Entity<WaultWallet>()
                .HasKey(p => new { p.Name, p.UserId });
            builder.Entity<WaultWallet>()
                .HasMany(p => p.Records).WithOne(p => p.WaultWallet).HasForeignKey(p => new { p.WaultWalletName, p.UserId });
            builder.Entity<WaultWallet>().HasOne(p => p.User).WithMany(p => p.WaultWallets).HasForeignKey(p => p.UserId);
            builder.Entity<ClaimDefinition>().HasOne(p => p.User).WithMany(p => p.ClaimDefinitions).HasForeignKey(p => p.UserId);
        }
    }
}
