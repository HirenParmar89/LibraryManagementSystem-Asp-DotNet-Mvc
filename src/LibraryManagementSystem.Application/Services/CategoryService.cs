using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Interfaces.Repositories;
using LibraryManagementSystem.Application.Interfaces.Services;
using LibraryManagementSystem.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace LibraryManagementSystem.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;
    private const string CategoriesCacheKey = "AllCategories";

    public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMemoryCache cache)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<ServiceResult<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
    {
        // Try to get from cache first
        if (_cache.TryGetValue(CategoriesCacheKey, out IEnumerable<CategoryDto>? categories) && categories != null)
        {
            return ServiceResult<IEnumerable<CategoryDto>>.Succeeded(categories);
        }

        // If not in cache, fetch from DB
        var entities = await _categoryRepository.GetAllAsync();
        categories = entities.Select(c => new CategoryDto(c.Id, c.Name)).ToList();

        // Save to cache for 5 minutes
        _cache.Set(CategoriesCacheKey, categories, TimeSpan.FromMinutes(5));

        return ServiceResult<IEnumerable<CategoryDto>>.Succeeded(categories);
    }

    public async Task<ServiceResult<CategoryDto>> GetCategoryByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) return ServiceResult<CategoryDto>.Failed("Category not found.");

        return ServiceResult<CategoryDto>.Succeeded(new CategoryDto(category.Id, category.Name));
    }

    public async Task<ServiceResult<Category>> CreateCategoryAsync(Category category)
    {
        if (await _categoryRepository.NameExistsAsync(category.Name))
            return ServiceResult<Category>.Failed("Category already exists.");

        await _categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        ClearCache(); // Invalidate cache
        return ServiceResult<Category>.Succeeded(category);
    }

    public async Task<ServiceResult<Category>> UpdateCategoryAsync(Category category)
    {
        if (await _categoryRepository.NameExistsAsync(category.Name, category.Id))
            return ServiceResult<Category>.Failed("Category already exists.");

        category.UpdatedAt = DateTime.UtcNow;
        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync();

        ClearCache(); // Invalidate cache
        return ServiceResult<Category>.Succeeded(category);
    }

    public async Task<ServiceResult> DeleteCategoryAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) return ServiceResult.Failed("Category not found.");

        category.IsActive = false;
        category.UpdatedAt = DateTime.UtcNow;
        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync();

        ClearCache(); // Invalidate cache
        return ServiceResult.Succeeded();
    }

    private void ClearCache()
    {
        _cache.Remove(CategoriesCacheKey);
    }
}