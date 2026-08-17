using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Services;

public class PublisherService : IPublisherService
{
    private readonly IPublisherRepository _publisherRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublisherService(IPublisherRepository publisherRepository, IUnitOfWork unitOfWork)
    {
        _publisherRepository = publisherRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<IEnumerable<PublisherDto>>> GetAllPublishersAsync()
    {
        var publishers = await _publisherRepository.GetAllAsync();
        var dtos = publishers.Select(p => new PublisherDto(p.Id, p.Name));
        return ServiceResult<IEnumerable<PublisherDto>>.Succeeded(dtos);
    }

    public async Task<ServiceResult<PublisherDto>> GetPublisherByIdAsync(Guid id)
    {
        var publisher = await _publisherRepository.GetByIdAsync(id);
        if (publisher == null) return ServiceResult<PublisherDto>.Failed("Publisher not found.");

        return ServiceResult<PublisherDto>.Succeeded(new PublisherDto(publisher.Id, publisher.Name));
    }

    public async Task<ServiceResult<Publisher>> CreatePublisherAsync(Publisher publisher)
    {
        if (await _publisherRepository.NameExistsAsync(publisher.Name))
            return ServiceResult<Publisher>.Failed("Publisher already exists.");

        await _publisherRepository.AddAsync(publisher);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult<Publisher>.Succeeded(publisher);
    }

    public async Task<ServiceResult<Publisher>> UpdatePublisherAsync(Publisher publisher)
    {
        if (await _publisherRepository.NameExistsAsync(publisher.Name, publisher.Id))
            return ServiceResult<Publisher>.Failed("Publisher already exists.");

        publisher.UpdatedAt = DateTime.UtcNow;
        _publisherRepository.Update(publisher);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult<Publisher>.Succeeded(publisher);
    }

    public async Task<ServiceResult> DeletePublisherAsync(Guid id)
    {
        var publisher = await _publisherRepository.GetByIdAsync(id);
        if (publisher == null) return ServiceResult.Failed("Publisher not found.");

        // Soft delete
        publisher.IsActive = false;
        publisher.UpdatedAt = DateTime.UtcNow;
        _publisherRepository.Update(publisher);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Succeeded();
    }
}