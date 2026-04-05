using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Persistence.Configurations
{
    /// <summary>
    /// EF Core mapping for the Agents table.
    /// </summary>
    public class AgentConfiguration : IEntityTypeConfiguration<Agent>
    {
        public void Configure(EntityTypeBuilder<Agent> entity)
        {
            entity.ToTable("Agents");
            entity.HasKey(e => e.AgentId);
            entity.Property(e => e.AgentId).ValueGeneratedNever();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.AgentCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.IsApproved).IsRequired();
            entity.Property(e => e.IsCommissionEligible).IsRequired();
            entity.Property(e => e.RegistrationSource).IsRequired();
            entity.Property(e => e.RegistrationStatus).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime2").IsRequired();
            entity.Property(e => e.ApprovedAt).HasColumnType("datetime2");
            entity.Property(e => e.JoinedAt).HasColumnType("datetime2");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");
            entity.HasIndex(e => e.UserId).IsUnique().HasDatabaseName("UQ_Agents_UserId");
            entity.HasIndex(e => e.AgentCode).IsUnique().HasDatabaseName("UQ_Agents_AgentCode");
        }
    }
}
