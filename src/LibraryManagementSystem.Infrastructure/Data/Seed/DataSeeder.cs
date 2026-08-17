using LibraryManagementSystem.Domain.Constants;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.Data;
using LibraryManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagementSystem.Infrastructure.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created and migrations are applied
            await context.Database.MigrateAsync();

            // 1. Seed Roles
            await SeedRolesAsync(roleManager);

            // 2. Seed SuperAdmin User
            await SeedSuperAdminAsync(userManager);

            // 3. Seed Sample Library Data
            await SeedSampleDataAsync(context);
        }
        catch (Exception ex)
        {
            // In a real app, log this exception
            Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { 
            RoleConstants.SuperAdmin, 
            RoleConstants.Admin, 
            RoleConstants.Librarian, 
            RoleConstants.Assistant, 
            RoleConstants.Member 
        };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    private static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager)
    {
        var adminEmail = "superadmin@library.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Super",
                LastName = "Admin",
                EmailConfirmed = true,
                IsActive = true
            };

            // NOTE: In production, read this from environment variables/secrets!
            var result = await userManager.CreateAsync(newAdmin, "SuperAdmin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, RoleConstants.SuperAdmin);
            }
        }
    }

    private static async Task SeedSampleDataAsync(ApplicationDbContext context)
    {
        // Only seed if the database is completely empty of books
        if (await context.Books.AnyAsync()) return;

        // Seed Categories
        var sciFiCat = new Category { Name = "Science Fiction", IsActive = true };
        var historyCat = new Category { Name = "History", IsActive = true };
        var techCat = new Category { Name = "Technology", IsActive = true };
        
        await context.Categories.AddRangeAsync(sciFiCat, historyCat, techCat);

        // Seed Authors
        var author1 = new Author { FirstName = "Isaac", LastName = "Asimov", IsActive = true };
        var author2 = new Author { FirstName = "Yuval Noah", LastName = "Harari", IsActive = true };
        var author3 = new Author { FirstName = "Robert C.", LastName = "Martin", IsActive = true };

        await context.Authors.AddRangeAsync(author1, author2, author3);

        // Seed Publishers
        var pub1 = new Publisher { Name = "Generic Sci-Fi Press", IsActive = true };
        var pub2 = new Publisher { Name = "Harper Collins", IsActive = true };
        var pub3 = new Publisher { Name = "Prentice Hall", IsActive = true };

        await context.Publishers.AddRangeAsync(pub1, pub2, pub3);

        await context.SaveChangesAsync(); // Save to generate IDs

        // Seed Books
        var book1 = new Book
        {
            ISBN = "978-0553293350",
            Title = "Foundation",
            AuthorId = author1.Id,
            CategoryId = sciFiCat.Id,
            PublisherId = pub1.Id,
            PublishedDate = new DateTime(1951, 5, 1),
            TotalCopies = 5,
            AvailableCopies = 5,
            IsActive = true,
            Price = 15.99m
        };

        var book2 = new Book
        {
            ISBN = "978-0062316097",
            Title = "Sapiens: A Brief History of Humankind",
            AuthorId = author2.Id,
            CategoryId = historyCat.Id,
            PublisherId = pub2.Id,
            PublishedDate = new DateTime(2014, 2, 10),
            TotalCopies = 10,
            AvailableCopies = 10,
            IsActive = true,
            Price = 25.00m
        };

        var book3 = new Book
        {
            ISBN = "978-0132350884",
            Title = "Clean Code: A Handbook of Agile Software Craftsmanship",
            AuthorId = author3.Id,
            CategoryId = techCat.Id,
            PublisherId = pub3.Id,
            PublishedDate = new DateTime(2008, 8, 1),
            TotalCopies = 3,
            AvailableCopies = 3,
            IsActive = true,
            Price = 45.00m
        };

        await context.Books.AddRangeAsync(book1, book2, book3);
        await context.SaveChangesAsync();

        // Seed Book Copies for Book 1
        var copies = new List<BookCopy>();
        for (int i = 1; i <= book1.TotalCopies; i++)
        {
            copies.Add(new BookCopy
            {
                BookId = book1.Id,
                AccessionNumber = $"ACC-{book1.ISBN}-{i:D3}",
                Barcode = $"BAR{book1.ISBN}{i:D3}",
                Condition = BookCondition.New,
                Status = BookCopyStatus.Available,
                IsAvailable = true,
                PurchaseDate = DateTime.UtcNow
            });
        }
        
        await context.BookCopies.AddRangeAsync(copies);
        await context.SaveChangesAsync();
    }
}