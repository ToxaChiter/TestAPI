using Application.Services;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.EventUC;

public class SetEventImageUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ImageService _imageService;

    public SetEventImageUseCase(IUnitOfWork unitOfWork, ImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _imageService = imageService;
    }

    public async Task ExecuteAsync(int id, IFormFile file)
    {
        var @event = await _unitOfWork.Events.GetByIdAsync(id);
        if (@event is null)
        {
            throw new NotFoundException("Event not found");
        }

        var filename = file.FileName;

        @event.ImagePath = filename;
        var updated = await _unitOfWork.Events.UpdateAsync(@event);
        if (updated is null)
        {
            throw new Exception("Event was not updated");
        }

        if (updated.ImagePath != filename)
        {
            throw new Exception("File names are not matching");
        }

        await _imageService.Upload(file);
    }
}
