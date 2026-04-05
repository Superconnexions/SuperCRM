using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Persistence.Configurations
{
    /// <summary>
    /// EF Core mapping for the UserProfiles table.
    /// </summary>
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> entity)
        {
            entity.ToTable("UserProfiles");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PhoneNo).HasMaxLength(50);
            entity.Property(e => e.MobileNo).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime2").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");
        }
    }
}
