using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceMarketplace.Domain.Entities;
using ServiceMarketplace.Domain.Enums;

namespace ServiceMarketplace.Infrastructure.Persistence.Configurations;

public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
{
    public void Configure(EntityTypeBuilder<ServiceRequest> builder)
    {
        builder.HasKey(r => r.Id);



        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasDefaultValue(RequestStatus.Pending);

        builder.Property(r => r.Latitude)
            .IsRequired();

        builder.Property(r => r.Longitude)
            .IsRequired();

        builder.Property(r => r.Address)
            .HasMaxLength(500);

        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("(CURRENT_TIMESTAMP)");

        builder.Property(r => r.UpdatedAt)
            .HasDefaultValueSql("(CURRENT_TIMESTAMP)");

        // Indexes
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.CustomerId);
        builder.HasIndex(r => r.ProviderId);
        builder.HasIndex(r => new { r.Latitude, r.Longitude });
    }
}
