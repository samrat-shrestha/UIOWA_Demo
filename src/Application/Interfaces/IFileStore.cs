using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IFileStore
{
    /// <summary>
    /// Saves the uploaded file and returns the relative path that was stored.
    /// </summary>
    Task<string> SaveAsync(IFormFile file, CancellationToken ct = default);
}