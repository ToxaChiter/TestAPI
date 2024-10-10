using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace TestAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly string _directory;
    private readonly ILogger<ImageController> _logger;

    public ImageController(ILogger<ImageController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        _directory = _configuration["ImageDirectory"]!;
    }

    [HttpGet("GetImage/{fileName}")]
    public IActionResult Get(string fileName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), _directory, fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        var lastModifiedTime = System.IO.File.GetLastWriteTimeUtc(filePath);

        var image = System.IO.File.OpenRead(filePath);

        return File(image, "image/jpeg", fileName, new DateTimeOffset(DateTime.UtcNow.AddDays(1)), new EntityTagHeaderValue($"\"{fileName} {lastModifiedTime}\""));
    }

    [HttpPut("UploadImage")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file selected");
        }

        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), _directory);

        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        var fileName = Path.GetFileName(file.FileName);
        var filePath = Path.Combine(uploadsPath, fileName);

        //if (System.IO.File.Exists(filePath))
        //{
        //    return Conflict("File already exists");
        //}

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        _logger.LogInformation("Uploaded image: {name}, size: {length}", fileName, file.Length);

        return Ok(new { FilePath = fileName });
    }

}








