using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Infrastructure.Data.Configurations;

public class FineConfiguration : IEntityTypeConfiguration<Fine>
{
    public void Configure(EntityTypeBuilder<Fine> builder)
    {
        builder.ToTable("Fines");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Amount).HasColumnType("decimal(18, 2)");
        builder.Property(f => f.PaidAmount).HasColumnType("decimal(18, 2)");
        builder.Property(f => f.RemainingAmount).HasColumnType("decimal(18, 2)");
        builder.Property(f => f.Reason).HasMaxLength(500).IsRequired();
        builder.Property(f => f.Notes).HasColumnType("nvarchar(max)");

        builder.HasOne(f => f.Loan)
            .WithMany(l => l.Fines)
            .HasForeignKey(f => f.LoanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Member)
            .WithMany(m => m.Fines)
            .HasForeignKey(f => f.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(f => new { f.MemberId, f.PaymentStatus });
    }
}