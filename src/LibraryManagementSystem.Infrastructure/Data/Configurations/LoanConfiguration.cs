using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Infrastructure.Data.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.IssuedByUserId).HasMaxLength(450).IsRequired();
        builder.Property(l => l.Notes).HasColumnType("nvarchar(max)");
        builder.Property(l => l.FineAmount).HasColumnType("decimal(18, 2)");

        builder.HasOne(l => l.BookCopy)
            .WithMany(bc => bc.Loans)
            .HasForeignKey(l => l.BookCopyId)
            .OnDelete(DeleteBehavior.Restrict); // Don't delete loan history if copy is deleted

        builder.HasOne(l => l.Member)
            .WithMany(m => m.Loans)
            .HasForeignKey(l => l.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => new { l.MemberId, l.Status });
        builder.HasIndex(l => l.DueDate);
    }
}