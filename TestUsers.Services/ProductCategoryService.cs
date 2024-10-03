﻿using TestUsers.Data.Models;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.ProductCategory;
using TestUsers.Services.Dtos.Validators.ProductCategories;
using TestUsers.Services.Interfaces.Services;
using TestUsers.Data;
using TestUsers.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace TestUsers.Services;

public class ProductCategoryService(DataContext db) : IProductCategoryService
{
    public async Task<List<ProductCategoryListItem>> GetListByParent(ProductCategoryGetListByParentRequest request, CancellationToken cancellationToken = default)
    {
        await new ProductCategoryGetListByParentRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        var productCategories = await db.ProductCategory
            .Where(pc => 
                pc.Name.Contains(request.Search) 
                && pc.ParentCategoryId == request.ParentCategoryId)
            .ToListAsync(cancellationToken);

        return productCategories
            .Select(pc => new ProductCategoryListItem(pc.Id, pc.Name, pc.ParentCategoryId))
            .ToList();
    }

    public async Task<List<ProductCategoryTreeItem>> GetTree(CancellationToken cancellationToken = default)
    {
        // Получаем все категории без использования Include
        var productCategories = await db.ProductCategory
            .Select(pc => new
            {
                pc.Id,
                pc.Name,
                pc.ParentCategoryId
            })
            .ToListAsync(cancellationToken);

        // Создаем словарь для быстрого доступа к категориям по их идентификаторам
        var categoryDict = productCategories.ToDictionary(pc => pc.Id, pc => new ProductCategoryTreeItem(pc.Id, pc.Name, pc.ParentCategoryId, []));

        // Список для хранения корневых элементов
        var rootCategories = new List<ProductCategoryTreeItem>();

        // Строим дерево категорий
        foreach (var category in productCategories)
            if (category.ParentCategoryId.HasValue) // Добавляем категорию как дочернюю к родительской категории
                if (categoryDict.TryGetValue(category.ParentCategoryId.Value, out var parentCategory))
                    parentCategory.ChildCategories.Add(categoryDict[category.Id]);
            else
                rootCategories.Add(categoryDict[category.Id]); // Если нет родительской категории, добавляем в корневые категории

        return rootCategories;
    }

    public async Task<BaseResponse> Create(ProductCategoryCreateRequest request, CancellationToken cancellationToken = default)
    {
        await new ProductCategoryCreateRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        if (await db.ProductCategory.AnyAsync(pc => pc.Name == request.Name, cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(ProductCategory.Name), nameof(ProductCategory)));
        await db.ProductCategory.AddAsync(new ProductCategory(request.Name, request.ParentCategoryId), cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Update(ProductCategoryUpdateRequest request, CancellationToken cancellationToken = default)
    {
        await new ProductCategoryUpdateRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        var productCategory = await db.ProductCategory.FindAsync([ request.Id ], cancellationToken)
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategory)));

        if (await db.ProductCategory.AnyAsync(pc => pc.Name == request.Name && request.Id != pc.Id, cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(ProductCategory.Name), nameof(ProductCategory)));

        productCategory.ParentCategoryId = request.ParentCategoryId;
        productCategory.Name = request.Name;

        await db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Delete(int id, CancellationToken cancellationToken = default)
    {
        var rowsRemoved = await db.ProductCategory.Where(pc => pc.Id == id).ExecuteDeleteAsync(cancellationToken);
        if (rowsRemoved == 0)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategory)));
        return new BaseResponse();
    }
}