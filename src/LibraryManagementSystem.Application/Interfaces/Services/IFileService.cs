namespace LibraryManagementSystem.Application.Interfaces.Services;

public interface IFileService
{
    /// <summary>
    /// Saves a file stream to the specified folder securely. Returns the relative URL path.
    /// </summary>
    Task<string?> SaveFileAsync(Stream? fileStream, string fileName, string[] allowedExtensions, string folderName, long maxSizeBytes = 2 * 1024 * 1024);
    
    /// <summary>
    /// Deletes a file from the server.
    /// </summary>
    void DeleteFile(string? relativeUrl);
}