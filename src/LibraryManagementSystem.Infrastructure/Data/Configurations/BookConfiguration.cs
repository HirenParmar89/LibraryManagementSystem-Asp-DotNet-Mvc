using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Infrastructure.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.ISBN).HasMaxLength(20).IsRequired();
        builder.Property(b => b.Title).HasMaxLength(500).IsRequired();
        builder.Property(b => b.Subtitle).HasMaxLength(500);
        builder.Property(b => b.Description).HasColumnType("nvarchar(max)");
        builder.Property(b => b.Edition).HasMaxLength(50);
        builder.Property(b => b.Language).HasMaxLength(50);
        builder.Property(b => b.ShelfLocation).HasMaxLength(50);
        builder.Property(b => b.CoverImageUrl).HasMaxLength(500);
        builder.Property(b => b.Price).HasColumnType("decimal(18, 2)");

        // Relationships
        builder.HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Publisher)
            .WithMany(p => p.Books)
            .HasForeignKey(b => b.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(b => b.ISBN).IsUnique();
        builder.HasIndex(b => b.Title);
    }
}