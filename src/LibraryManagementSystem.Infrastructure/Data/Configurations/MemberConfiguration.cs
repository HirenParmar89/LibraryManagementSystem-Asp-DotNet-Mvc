using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Infrastructure.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.MembershipNumber).HasMaxLength(50).IsRequired();
        builder.Property(m => m.ApplicationUserId).HasMaxLength(450).IsRequired();
        builder.Property(m => m.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(m => m.LastName).HasMaxLength(100).IsRequired();
        builder.Property(m => m.Email).HasMaxLength(256).IsRequired();
        builder.Property(m => m.Phone).HasMaxLength(50);
        builder.Property(m => m.Address).HasMaxLength(500);
        builder.Property(m => m.ProfileImageUrl).HasMaxLength(500);

        // 1-to-1 Relationship with ApplicationUser
        builder.HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<Member>(m => m.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => m.MembershipNumber).IsUnique();
        builder.HasIndex(m => m.ApplicationUserId).IsUnique();
    }
}