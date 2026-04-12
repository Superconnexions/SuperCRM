using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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

        public DbSet<Country> Countries => Set<Country>();
        public DbSet<Region> Regions => Set<Region>();
        public DbSet<City> Cities => Set<City>();

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

            builder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("UserProfiles");

                entity.HasKey(e => e.UserId);

                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.PhoneNo).HasMaxLength(50);
                entity.Property(e => e.MobileNo).HasMaxLength(50);

                entity.HasOne<ApplicationUser>()
                      .WithOne()
                      .HasForeignKey<UserProfile>(e => e.UserId)
                      .HasConstraintName("FK_UserProfiles_User");

                entity.HasOne<Country>()
                      .WithMany()
                      .HasForeignKey(e => e.CountryId)
                      .HasConstraintName("FK_UserProfiles_Country");

                entity.HasOne<Region>()
                      .WithMany()
                      .HasForeignKey(e => e.RegionId)
                      .HasConstraintName("FK_UserProfiles_Region");

                entity.HasOne<City>()
                      .WithMany()
                      .HasForeignKey(e => e.CityId)
                      .HasConstraintName("FK_UserProfiles_City");

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(e => e.UpdatedByUserId)
                      .HasConstraintName("FK_UserProfiles_UpdatedBy");
            });


            builder.Entity<UserAddress>(entity =>
            {
                entity.ToTable("UserAddresses");

                entity.HasKey(e => e.UserAddressId);

                entity.Property(e => e.HouseNo).HasMaxLength(100);
                entity.Property(e => e.RoadName).HasMaxLength(150);
                entity.Property(e => e.AddressLine1).HasMaxLength(200);
                entity.Property(e => e.AddressLine2).HasMaxLength(200);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.PostCode).HasMaxLength(20);

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .HasConstraintName("FK_UserAddresses_User");

                entity.HasOne<Country>()
                      .WithMany()
                      .HasForeignKey(e => e.CountryId)
                      .HasConstraintName("FK_UserAddresses_Country");

                entity.HasOne<Region>()
                      .WithMany()
                      .HasForeignKey(e => e.RegionId)
                      .HasConstraintName("FK_UserAddresses_Region");

                entity.HasOne<City>()
                      .WithMany()
                      .HasForeignKey(e => e.CityId)
                      .HasConstraintName("FK_UserAddresses_City");

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(e => e.UpdatedByUserId)
                      .HasConstraintName("FK_UserAddresses_UpdatedBy");
            });
            // Added for Agent Registration------
            builder.ApplyConfigurationsFromAssembly(typeof(SuperCrmDbContext).Assembly);
        }
    }
}
