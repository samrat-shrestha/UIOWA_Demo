using Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.FileStorage;

public class LocalFileStore : IFileStore
{
    private readonly string _root;
    private readonly ILogger<LocalFileStore> _logger;

    public LocalFileStore(IHostEnvironment env, ILogger<LocalFileStore> logger)
    {
        _root = Path.Combine(env.ContentRootPath, "Receipts");
        Directory.CreateDirectory(_root);
        _logger = logger;
    }

    public async Task<string> SaveAsync(IFormFile file, CancellationToken ct = default)
    {
        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var path = Path.Combine(_root, fileName);

        await using var stream = File.Create(path);
        await file.CopyToAsync(stream, ct);

        _logger.LogInformation("Saved receipt file to {Path}", path);
        return Path.Combine("Receipts", fileName); // relative path for persistence
    }
}