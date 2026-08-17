using LibraryManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace LibraryManagementSystem.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly string _webRootPath;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
        _webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    }

    public async Task<string?> SaveFileAsync(Stream? fileStream, string fileName, string[] allowedExtensions, string folderName, long maxSizeBytes = 2 * 1024 * 1024)
    {
        if (fileStream == null || fileStream.Length == 0) return null;

        // Validate file size
        if (fileStream.Length > maxSizeBytes)
        {
            throw new InvalidOperationException($"File size exceeds the {maxSizeBytes / 1024 / 1024}MB limit.");
        }

        // Validate extension
        var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
        {
            throw new InvalidOperationException($"File type {fileExtension} is not allowed.");
        }

        // Create folder if it doesn't exist
        var uploadsFolder = Path.Combine(_webRootPath, "uploads", folderName);
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Generate unique filename to prevent overwrites and path traversal attacks
        var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var destinationStream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(destinationStream);
        }

        // Return relative URL path for database storage
        return $"/uploads/{folderName}/{uniqueFileName}";
    }

    public void DeleteFile(string? relativeUrl)
    {
        if (string.IsNullOrEmpty(relativeUrl)) return;

        // Convert relative URL to physical path
        var filePath = Path.Combine(_webRootPath, relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}