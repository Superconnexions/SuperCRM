using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Persistence.Configurations
{
    /// <summary>
    /// EF Core mapping for the UserAddresses table.
    /// </summary>
    public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> entity)
        {
            entity.ToTable("UserAddresses");
            entity.HasKey(e => e.UserAddressId);
            entity.Property(e => e.UserAddressId).ValueGeneratedNever();
            entity.Property(e => e.AddressType).IsRequired();
            entity.Property(e => e.HouseNo).HasMaxLength(100);
            entity.Property(e => e.RoadName).HasMaxLength(150);
            entity.Property(e => e.AddressLine1).HasMaxLength(200);
            entity.Property(e => e.AddressLine2).HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.PostCode).HasMaxLength(20);
            entity.Property(e => e.IsDefault).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime2").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");
        }
    }
}
