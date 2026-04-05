using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.Identity;
using SuperCRM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperCRM.Persistence.DbContexts
{
    public class SuperCrmDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public SuperCrmDbContext(DbContextOptions<SuperCrmDbContext> options): base(options)
        {
        }

        public DbSet<Provider> Providers => Set<Provider>();
        public DbSet<ProductVariantType> ProductVariantTypes => Set<ProductVariantType>();

        // For Agent Registration-----------
        public DbSet<Agent> Agents => Set<Agent>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<UserAddress> UserAddresses => Set<UserAddress>();
        // END Agent Registration

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Map to existing SCHEMA_V1_BASELINE tables
            builder.Entity<ApplicationUser>().ToTable("AspNetUsers");
            builder.Entity<ApplicationRole>().ToTable("AspNetRoles");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>>().ToTable("AspNetUserRoles");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>>().ToTable("AspNetUserClaims");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>>().ToTable("AspNetUserLogins");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>().ToTable("AspNetUserTokens");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>>().ToTable("AspNetRoleClaims");

            // Provdier table

            builder.Entity<Provider>(entity =>
            {
                entity.ToTable("Providers");

                entity.HasKey(e => e.ProviderId);

                entity.Property(e => e.ProviderId)
                      .ValueGeneratedNever();

                entity.Property(e => e.ProviderName)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(e => e.ContactEmail)
                      .HasMaxLength(150);

                entity.Property(e => e.ContactPhone)
                      .HasMaxLength(50);

                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime2");

                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("datetime2");

                entity.Property(e => e.IsActive)
                      .IsRequired();

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(e => e.UpdatedByUserId)
                      .HasConstraintName("FK_Providers_UpdatedBy");
            });

            // ProductVariantType

            builder.Entity<ProductVariantType>(entity =>
            {
                entity.ToTable("ProductVariantTypes");

                entity.HasKey(e => e.TypeCode);

                entity.Property(e => e.TypeCode)
                      .HasMaxLength(50)
                      .IsRequired()
                      .ValueGeneratedNever();

                entity.Property(e => e.TypeValue)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.DisplayOrder)
                      .IsRequired();

                entity.Property(e => e.IsActive)
                      .IsRequired();

                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime2")
                      .IsRequired();

                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("datetime2");

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(e => e.UpdatedByUserId)
                      .HasConstraintName("FK_ProductVariantTypes_UpdatedBy");
            });

            // Added for Agent Registration------
            builder.ApplyConfigurationsFromAssembly(typeof(SuperCrmDbContext).Assembly);
        }
    }
}
