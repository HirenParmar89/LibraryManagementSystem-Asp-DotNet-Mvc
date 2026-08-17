using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Infrastructure.Data.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("Authors");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(a => a.LastName).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Biography).HasColumnType("nvarchar(max)");
        builder.Property(a => a.Country).HasMaxLength(100);
        builder.Property(a => a.Email).HasMaxLength(256);
        builder.Property(a => a.Website).HasMaxLength(256);

        // Index for fast searching
        builder.HasIndex(a => new { a.FirstName, a.LastName });
    }
}