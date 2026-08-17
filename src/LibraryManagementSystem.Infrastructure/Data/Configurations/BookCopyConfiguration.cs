using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Infrastructure.Data.Configurations;

public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
{
    public void Configure(EntityTypeBuilder<BookCopy> builder)
    {
        builder.ToTable("BookCopies");
        builder.HasKey(bc => bc.Id);

        builder.Property(bc => bc.AccessionNumber).HasMaxLength(50).IsRequired();
        builder.Property(bc => bc.Barcode).HasMaxLength(50).IsRequired();
        builder.Property(bc => bc.ShelfLocation).HasMaxLength(50);
        builder.Property(bc => bc.Price).HasColumnType("decimal(18, 2)");

        builder.HasOne(bc => bc.Book)
            .WithMany(b => b.BookCopies)
            .HasForeignKey(bc => bc.BookId)
            .OnDelete(DeleteBehavior.Cascade); // If a book is deleted, its copies are deleted

        builder.HasIndex(bc => bc.Barcode).IsUnique();
        builder.HasIndex(bc => bc.AccessionNumber).IsUnique();
    }
}