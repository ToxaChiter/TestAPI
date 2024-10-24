using Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Application.Services;

public class ImageService
{
    private readonly IConfiguration _configuration;
    private readonly string _directory;
    private readonly ILogger<ImageService> _logger;

    public ImageService(IConfiguration configuration, ILogger<ImageService> logger)
    {
        _logger = logger;
        _configuration = configuration;

        _directory = _configuration["ImageDirectory"]!;
    }

    public (FileStream fileStream, string contentType, string fileDownloadName, DateTimeOffset lastModified, EntityTagHeaderValue entityTag) Get(string fileName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), _directory, fileName);

        if (!File.Exists(filePath))
        {
            throw new NotFoundException($"{fileName} not found");
        }

        var lastModifiedTime = File.GetLastWriteTimeUtc(filePath);

        var image = File.OpenRead(filePath);

        return (image, "image/jpeg", fileName, new DateTimeOffset(DateTime.UtcNow.AddDays(1)), new EntityTagHeaderValue($"\"{fileName} {lastModifiedTime}\""));
    }

    public async Task Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new BadRequestException("No file selected");
        }

        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), _directory);

        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        var fileName = Path.GetFileName(file.FileName);
        var filePath = Path.Combine(uploadsPath, fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        _logger.LogInformation("Uploaded image: {name}, size: {length}", fileName, file.Length);
    }
}