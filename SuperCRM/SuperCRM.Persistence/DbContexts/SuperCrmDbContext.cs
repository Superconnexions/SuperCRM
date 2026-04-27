using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperCRM.Domain.Entities;
//using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SuperCRM.Persistence.DbContexts
{
    public class SuperCrmDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public SuperCrmDbContext(DbContextOptions<SuperCrmDbContext> options): base(options)
        {
        }

        // Start Master Setup -----------
        public DbSet<Provider> Providers => Set<Provider>();
        public DbSet<ProductVariantType> ProductVariantTypes => Set<ProductVariantType>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<SalesUnit> SalesUnits => Set<SalesUnit>();

        // END Mater Setup 

        // For Agent Registration-----------
        public DbSet<Agent> Agents => Set<Agent>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<UserAddress> UserAddresses => Set<UserAddress>();
        // END Agent Registration

        // Start Product Management -----------
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();

        public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
        public DbSet<ProviderProduct> ProviderProducts { get; set; }

        // END Product Management

        public DbSet<Country> Countries => Set<Country>();
        public DbSet<Region> Regions => Set<Region>();
        public DbSet<City> Cities => Set<City>();

        public DbSet<ProductBaseCommission> ProductBaseCommissions { get; set; }
        public DbSet<ProductBaseCommissionHistory> ProductBaseCommissionHistories { get; set; }

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

                entity.Property(e => e.ProviderUrl)
                      .HasMaxLength(100);


                entity.Property(e => e.ProviderAddress)
                      .HasMaxLength(200);

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


            // Start ProductVariant

            builder.Entity<ProductVariant>(entity =>
            {
                entity.ToTable("ProductVariants");

                entity.HasKey(e => e.ProductVariantId);

                entity.Property(e => e.ProductVariantId)
                      .ValueGeneratedNever();

                entity.Property(e => e.VariantCode)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.VariantTypeCode)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.VariantName)
                      .HasMaxLength(200)
                      .IsRequired();

                //entity.Property(e => e.DisplayStyle)
                //      .IsRequired();

                entity.Property(e => e.DisplayStyle)
                      .HasConversion<byte>()
                      .IsRequired();

                entity.Property(e => e.DisplayOrder)
                      .IsRequired();

                entity.Property(e => e.BasePrice)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime2")
                      .IsRequired();

                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("datetime2");

                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .HasConstraintName("FK_ProductVariants_Product");

                entity.HasOne(e => e.VariantType)
                      .WithMany()
                      .HasForeignKey(e => e.VariantTypeCode)
                      .HasPrincipalKey(e => e.TypeCode)
                      .HasConstraintName("FK_ProductVariants_VariantType");

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(e => e.UpdatedByUserId)
                      .HasConstraintName("FK_ProductVariants_UpdatedBy");

                entity.HasIndex(e => new { e.ProductId, e.VariantCode })
                      .IsUnique()
                      .HasDatabaseName("UQ_ProductVariants_Product_VariantCode");
            });

            // END ProductVariant

            // Start ProviderProduct

            builder.Entity<ProviderProduct>(entity =>
            {
                entity.ToTable("ProviderProducts");
                entity.HasKey(x => x.ProviderProductId);

                entity.Property(x => x.ProductCode)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.ProductName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.CreatedAt)
                    .HasColumnType("datetime2")
                    .IsRequired();

                entity.Property(x => x.UpdatedAt)
                    .HasColumnType("datetime2");

                entity.HasOne(x => x.Provider)
                    .WithMany()
                    .HasForeignKey(x => x.ProviderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Product)
                    .WithMany()
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // END ProviderProduct

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

            // Start ProductCategory

            builder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("ProductCategories");

                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryId)
                      .ValueGeneratedNever();

                entity.Property(e => e.CategoryCode)
                      .HasMaxLength(50);

                entity.Property(e => e.CategoryName)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(e => e.CategoryImageUrl)
                      .HasMaxLength(500);

                entity.Property(e => e.DisplayNotes)
                      .HasMaxLength(500);

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
                      .HasConstraintName("FK_ProductCategories_UpdatedBy");
            });


            // Master ProductCategory


            // Strt SalesUnit

            builder.Entity<SalesUnit>(entity =>
            {
                entity.ToTable("SalesUnits");

                entity.HasKey(e => e.SalesUnitId);

                entity.Property(e => e.SalesUnitId)
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.UnitCode)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.UnitName)
                      .HasMaxLength(100)
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
                      .HasConstraintName("FK_SalesUnits_UpdatedBy");
            });
            // END Master SalesUnit


            // Start Commission Setup

            builder.Entity<ProductBaseCommission>(entity =>
            {
                entity.ToTable("ProductBaseCommission");
                entity.HasKey(x => x.ProductBaseCommissionId);

                entity.Property(x => x.ProductBaseCommissionId).ValueGeneratedNever();
                entity.Property(x => x.ProductId).IsRequired();
                entity.Property(x => x.CommissionType).HasConversion<byte>().IsRequired();
                entity.Property(x => x.FixedAmount).HasColumnType("decimal(18,2)");
                entity.Property(x => x.Percentage).HasColumnType("decimal(9,4)");
                entity.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(x => x.EffectiveFrom).HasColumnType("datetime2");
                entity.Property(x => x.EffectiveTo).HasColumnType("datetime2");
                entity.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
                entity.Property(x => x.CreatedByUserId).IsRequired();
                entity.Property(x => x.UpdatedAt).HasColumnType("datetime2");
                entity.Property(x => x.UpdatedByUserId);

                entity.HasOne(x => x.Product)
                    .WithMany()
                    .HasForeignKey(x => x.ProductId)
                    .HasConstraintName("FK_ProductBaseCommission_Product")
                    .OnDelete(DeleteBehavior.Restrict);

                //entity.HasOne(x => x.CreatedByUserId)
                //    .WithMany()
                //    .HasForeignKey(x => x.CreatedByUserId)
                //    .HasConstraintName("FK_ProductBaseCommission_CreatedBy")
                //    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(e => e.CreatedByUserId)
                      .HasConstraintName("FK_ProductBaseCommission_CreatedBy");

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(e => e.UpdatedByUserId)
                      .HasConstraintName("FK_ProductBaseCommission_UpdatedBy");

                //entity.HasOne(x => x.UpdatedByUserId)
                //    .WithMany()
                //    .HasForeignKey(x => x.UpdatedByUserId)
                //    .HasConstraintName("FK_ProductBaseCommission_UpdatedBy")
                //    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(x => x.Histories)
                    .WithOne(x => x.ProductBaseCommission)
                    .HasForeignKey(x => x.ProductBaseCommissionId)
                    .HasConstraintName("FK_ProductBaseCommissionHistory_Base")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.ProductId);
                entity.HasIndex(x => x.IsActive);
                entity.HasIndex(x => x.EffectiveFrom);
                entity.HasIndex(x => x.EffectiveTo);
            });

            builder.Entity<ProductBaseCommissionHistory>(entity =>
            {
                entity.ToTable("ProductBaseCommissionHistory");
                entity.HasKey(x => x.HistoryId);

                entity.Property(x => x.HistoryId).ValueGeneratedNever();
                entity.Property(x => x.ProductBaseCommissionId).IsRequired();
                entity.Property(x => x.ProductId).IsRequired();
                entity.Property(x => x.OldCommissionType).HasConversion<byte?>();
                entity.Property(x => x.OldFixedAmount).HasColumnType("decimal(18,2)");
                entity.Property(x => x.OldPercentage).HasColumnType("decimal(9,4)");
                entity.Property(x => x.NewCommissionType).HasConversion<byte?>();
                entity.Property(x => x.NewFixedAmount).HasColumnType("decimal(18,2)");
                entity.Property(x => x.NewPercentage).HasColumnType("decimal(9,4)");
                entity.Property(x => x.ChangedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()").IsRequired();
                entity.Property(x => x.ChangedByUserId).IsRequired();
                entity.Property(x => x.Note).HasMaxLength(500);
                entity.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
                entity.Property(x => x.CreatedByUserId).IsRequired();

                entity.HasOne(x => x.Product)
                    .WithMany()
                    .HasForeignKey(x => x.ProductId)
                    .HasConstraintName("FK_ProductBaseCommissionHistory_Product")
                    .OnDelete(DeleteBehavior.Restrict);

                //entity.HasOne(x => x.ChangedByUserId)
                //    .WithMany()
                //    .HasForeignKey(x => x.ChangedByUserId)
                //    .HasConstraintName("FK_ProductBaseCommissionHistory_ChangedBy")
                //    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<ApplicationUser>()
                     .WithMany()
                     .HasForeignKey(e => e.ChangedByUserId)
                     .HasConstraintName("FK_ProductBaseCommissionHistory_ChangedBy");


                //entity.HasOne(x => x.CreatedByUserId)
                //    .WithMany()
                //    .HasForeignKey(x => x.CreatedByUserId)
                //    .HasConstraintName("FK_ProductBaseCommissionHistory_CreatedBy")
                //    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<ApplicationUser>()
                     .WithMany()
                     .HasForeignKey(e => e.CreatedByUserId)
                     .HasConstraintName("FK_ProductBaseCommissionHistory_CreatedBy");


                entity.HasIndex(x => x.ProductBaseCommissionId);
                entity.HasIndex(x => x.ProductId);
                entity.HasIndex(x => x.ChangedAt);
            });

            // END Commission Setup


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
            // END Added for Agent Registration------

            // Start Product Management

            builder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.ProductId)
                      .ValueGeneratedNever();

                entity.Property(e => e.ProductCode)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.ProductName)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(e => e.ProductDisplayName)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(e => e.ProductType)
                      .HasConversion<byte>()
                      .IsRequired();

                entity.Property(e => e.CustomerType)
                      .HasConversion<byte?>();

                entity.Property(e => e.ProductDescription)
                      .HasMaxLength(1000);

                entity.Property(e => e.CurrencyCode)
                      .HasMaxLength(10)
                      .IsRequired();

                entity.Property(e => e.BasePriceType)
                      .HasConversion<byte>()
                      .IsRequired();

                entity.Property(e => e.BasePrice)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.MonthlyInstallmentAmount)
                    .HasColumnType("decimal(18,2)");


                entity.Property(e => e.DisplayOrder)
                      .IsRequired();

                entity.Property(e => e.NoOfInstallment);
                      //.IsRequired();

                entity.Property(e => e.DownPaymentAmount)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.ProductDisplayNotes)
                      .HasMaxLength(1000);

                entity.Property(e => e.PaymentNotes)
                      .HasMaxLength(1000);

                entity.Property(e => e.Remarks)
                      .HasMaxLength(1000);

                entity.Property(e => e.SalesUnitCode)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime2")
                      .IsRequired();

                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("datetime2");

                entity.HasOne(e => e.Category)
                      .WithMany()
                      .HasForeignKey(e => e.CategoryId)
                      .HasConstraintName("FK_Products_Category");

                entity.HasOne(e => e.SalesUnit)
                      .WithMany()
                      .HasForeignKey(e => e.SalesUnitId)
                      .HasConstraintName("FK_Products_SalesUnit");

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(e => e.UpdatedByUserId)
                      .HasConstraintName("FK_Products_UpdatedBy");

                entity.HasMany(e => e.ProductImages)
                      .WithOne(e => e.Product)
                      .HasForeignKey(e => e.ProductId)
                      .HasConstraintName("FK_ProductImages_Product");

                entity.HasIndex(e => e.SalesUnitId)
                      .HasDatabaseName("IX_Products_SalesUnitId");

                entity.HasIndex(e => e.ProductCode)
                      .IsUnique()
                      .HasDatabaseName("UQ_Products_ProductCode");
            });

            builder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("ProductImages");

                entity.HasKey(e => e.ProductImageId);

                entity.Property(e => e.ProductImageId)
                      .ValueGeneratedNever();

                entity.Property(e => e.ImageUrl)
                      .HasMaxLength(500);

                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime2")
                      .IsRequired();
            });

            // END Product Management


            builder.ApplyConfigurationsFromAssembly(typeof(SuperCrmDbContext).Assembly);
        }
    }
}
