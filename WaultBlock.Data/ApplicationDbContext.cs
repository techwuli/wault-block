using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WaultBlock.Models;

namespace WaultBlock.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ClaimDefinition> ClaimDefinitions { get; set; }

        public virtual DbSet<CredentialSchema> CredentialSchemas { get; set; }

        public virtual DbSet<UserIndyClaim> UserIndyClaims { get; set; }

        public virtual DbSet<WalletData> WalletDatas { get; set; }

        public void UpdateEntity<T>(T entityToUpdate) where T : class
        {
            Set<T>().Attach(entityToUpdate);
            Entry(entityToUpdate).State = EntityState.Modified;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<WalletData>()
                .HasKey(p => new { p.Name, p.UserId });

            builder.Entity<WalletData>().HasOne(p => p.User).WithMany(p => p.WalletDatas).HasForeignKey(p => p.UserId);
            builder.Entity<ClaimDefinition>().HasOne(p => p.User).WithMany(p => p.ClaimDefinitions).HasForeignKey(p => p.UserId);
            builder.Entity<ClaimDefinition>().HasOne(p => p.CredentialSchema).WithMany(p => p.ClaimDefinitions).HasForeignKey(p => p.CredentialSchemaId);
            builder.Entity<ClaimDefinition>().HasMany(p => p.UserIndyClaims).WithOne(p => p.ClaimDefinition).HasForeignKey(p => p.ClaimDefinitionId);
            builder.Entity<CredentialSchema>().HasOne(p => p.User).WithMany(p => p.CredentialSchemas).HasForeignKey(p => p.UserId);
        }
    }
}
