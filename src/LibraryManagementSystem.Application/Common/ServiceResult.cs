namespace LibraryManagementSystem.Application.Common;

public class ServiceResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string>? ValidationErrors { get; set; }

    public static ServiceResult Succeeded() => new() { Success = true };
    
    public static ServiceResult Failed(string errorMessage) => new() { Success = false, ErrorMessage = errorMessage };
    
    public static ServiceResult Failed(Dictionary<string, string> validationErrors) => new() 
    { 
        Success = false, 
        ValidationErrors = validationErrors 
    };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }

    public static ServiceResult<T> Succeeded(T data) => new() { Success = true, Data = data };
    
    public new static ServiceResult<T> Failed(string errorMessage) => new() { Success = false, ErrorMessage = errorMessage };
}