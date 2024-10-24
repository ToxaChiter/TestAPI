using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    private readonly ImageService _imageService;

    public ImageController(ImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpGet("GetImage/{fileName}")]
    public IActionResult Get(string fileName)
    {
        var (fileStream, contentType, fileDownloadName, lastModified, entityTag) = _imageService.Get(fileName);

        return File(fileStream, contentType, fileDownloadName, lastModified, entityTag);
    }

    [HttpPut("UploadImage")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        await _imageService.Upload(file);
        return Ok();
    }
}