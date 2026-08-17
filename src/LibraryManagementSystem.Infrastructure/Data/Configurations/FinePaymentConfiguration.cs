using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Infrastructure.Data.Configurations;

public class FinePaymentConfiguration : IEntityTypeConfiguration<FinePayment>
{
    public void Configure(EntityTypeBuilder<FinePayment> builder)
    {
        builder.ToTable("FinePayments");
        builder.HasKey(fp => fp.Id);

        builder.Property(fp => fp.Amount).HasColumnType("decimal(18, 2)");
        builder.Property(fp => fp.ReceivedByUserId).HasMaxLength(450).IsRequired();
        builder.Property(fp => fp.TransactionReference).HasMaxLength(100);
        builder.Property(fp => fp.Notes).HasColumnType("nvarchar(max)");

        builder.HasOne(fp => fp.Fine)
            .WithMany(f => f.FinePayments)
            .HasForeignKey(fp => fp.FineId)
            .OnDelete(DeleteBehavior.Cascade); // If a fine is deleted, its payments are deleted
    }
}