using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceMarketplace.Domain.Entities;
using ServiceMarketplace.Domain.Enums;

namespace ServiceMarketplace.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);



        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(191);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Subscription)
            .HasConversion<string>()
            .HasDefaultValue(SubscriptionTier.Free);

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("(CURRENT_TIMESTAMP)");

        builder.Property(u => u.UpdatedAt)
            .HasDefaultValueSql("(CURRENT_TIMESTAMP)");

        // Relationships
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.UserPermissions)
            .WithOne(up => up.User)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.CreatedRequests)
            .WithOne(r => r.Customer)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.AcceptedRequests)
            .WithOne(r => r.Provider)
            .HasForeignKey(r => r.ProviderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(u => u.Employer)
            .WithMany(u => u.Employees)
            .HasForeignKey(u => u.EmployerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
