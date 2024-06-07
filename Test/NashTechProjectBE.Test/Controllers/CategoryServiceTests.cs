using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NashTechProjectBE.Application.Common.Interfaces;
using NashTechProjectBE.Application.Common.Models;
using NashTechProjectBE.Application.Common.ViewModel;
using NashTechProjectBE.Application.Services;
using NashTechProjectBE.Domain.Entities;
using NUnit.Framework;

namespace NashTechProjectBE.Application.Tests.Services
{
	[TestFixture]
	public class CategoryServiceTests
	{
		private Mock<IGenericRepository<Category>> _categoryRepositoryMock;
		private Mock<IMapper> _mapperMock;
		private CategoryService _categoryService;

		[SetUp]
		public void SetUp()
		{
			_categoryRepositoryMock = new Mock<IGenericRepository<Category>>();
			_mapperMock = new Mock<IMapper>();
			_categoryService = new CategoryService(_categoryRepositoryMock.Object, _mapperMock.Object);
		}

		[Test]
		public async Task CreateAsync_ShouldReturnBadRequest_WhenCategoryNameExists()
		{
			// Arrange
			var categoryForm = new CategoryCreateUpdateVM { Name = "ExistingCategory" };
			var existingCategories = new List<Category>
			{
				new Category { Name = categoryForm.Name }
			}.AsQueryable();

			_categoryRepositoryMock.Setup(repo => repo.FindByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
				.Returns(new TestAsyncEnumerable<Category>(existingCategories));

			// Act
			var result = await _categoryService.CreateAsync(categoryForm);

			// Assert
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
			result.Message.Should().Be($"There's already category with name: {categoryForm.Name}");
		}

		[Test]
		public async Task CreateAsync_ShouldReturnOk_WhenCategorySuccessfullyCreated()
		{
			// Arrange
			var categoryForm = new CategoryCreateUpdateVM { Name = "NewCategory", CreateUpdateUserId = Guid.NewGuid() };
			var emptyCategoryList = new List<Category>().AsQueryable();

			_categoryRepositoryMock.Setup(repo => repo.FindByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
				.Returns(new TestAsyncEnumerable<Category>(emptyCategoryList));

			_mapperMock.Setup(mapper => mapper.Map<Category>(categoryForm))
				.Returns(new Category { Name = categoryForm.Name });

			_categoryRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Category>(), categoryForm.CreateUpdateUserId))
				.ReturnsAsync(true);

			// Act
			var result = await _categoryService.CreateAsync(categoryForm) as Result;

			// Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			result.Message.Should().Contain("Category added, Id:");
		}

		[Test]
		public async Task UpdateAsync_ShouldReturnBadRequest_WhenCategoryNameExists()
		{
			// Arrange
			var categoryForm = new CategoryCreateUpdateVM { Name = "ExistingCategory" };
			var existingCategories = new List<Category>
			{
				new Category { Name = categoryForm.Name }
			}.AsQueryable();

			_categoryRepositoryMock.Setup(repo => repo.FindByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
				.Returns(new TestAsyncEnumerable<Category>(existingCategories));

			// Act
			var result = await _categoryService.UpdateAsync(Guid.NewGuid(), categoryForm);

			// Assert
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
			result.Message.Should().Be($"There's already category with name: {categoryForm.Name}");
		}

		[Test]
		public async Task UpdateAsync_ShouldReturnOk_WhenCategorySuccessfullyUpdated()
		{
			// Arrange
			var categoryForm = new CategoryCreateUpdateVM { Name = "UpdatedCategory", CreateUpdateUserId = Guid.NewGuid() };
			var emptyCategoryList = new List<Category>().AsQueryable();
			var categoryId = Guid.NewGuid();

			_categoryRepositoryMock.Setup(repo => repo.FindByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
				.Returns(new TestAsyncEnumerable<Category>(emptyCategoryList));

			_mapperMock.Setup(mapper => mapper.Map<Category>(categoryForm))
				.Returns(new Category { Name = categoryForm.Name, Id = categoryId });

			_categoryRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Category>(), categoryForm.CreateUpdateUserId))
				.ReturnsAsync(true);

			// Act
			var result = await _categoryService.UpdateAsync(categoryId, categoryForm);

			// Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			result.Message.Should().Contain("Category updated, Id:");
		}

		[Test]
		public async Task DeleteAsync_ShouldReturnNotFound_WhenCategoryDoesNotExist()
		{
			// Arrange
			var categoryId = Guid.NewGuid();
			var emptyCategoryList = new List<Category>().AsQueryable();

			_categoryRepositoryMock.Setup(repo => repo.FindByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
				.Returns(new TestAsyncEnumerable<Category>(emptyCategoryList));

			// Act
			var result = await _categoryService.DeleteAsync(categoryId);

			// Assert
			result.StatusCode.Should().Be(HttpStatusCode.NotFound);
			result.Message.Should().Be($"There's no category to delete with Id: {categoryId} ");
		}

		[Test]
		public async Task DeleteAsync_ShouldReturnOk_WhenCategorySuccessfullyDeleted()
		{
			// Arrange
			var categoryId = Guid.NewGuid();
			var category = new Category { Id = categoryId, Name = "ToDeleteCategory" };
			var existingCategories = new List<Category>
			{
				category
			}.AsQueryable();

			_categoryRepositoryMock.Setup(repo => repo.FindByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
				.Returns(new TestAsyncEnumerable<Category>(existingCategories));

			_categoryRepositoryMock.Setup(repo => repo.DeleteAsync(category))
				.ReturnsAsync(true);

			// Act
			var result = await _categoryService.DeleteAsync(categoryId);

			// Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			result.Message.Should().Be($"Book deleted, Id: {category.Id}");
		}

		[Test]
		public async Task GetCategoryByIdAsync_ShouldReturnCategoryDto_WhenCategoryExists()
		{
			// Arrange
			var categoryId = Guid.NewGuid();
			var category = new Category { Id = categoryId, Name = "ExistingCategory" };
			var categoryDto = new CategoryDto { Id = categoryId, Name = "ExistingCategory" };
			var existingCategories = new List<Category>
			{
				category
			}.AsQueryable();

			_categoryRepositoryMock.Setup(repo => repo.FindByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
				.Returns(new TestAsyncEnumerable<Category>(existingCategories));

			_mapperMock.Setup(mapper => mapper.Map<CategoryDto>(category))
				.Returns(categoryDto);

			// Act
			var result = await _categoryService.GetCategoryByIdAsync(categoryId);

			// Assert
			result.Should().BeEquivalentTo(categoryDto);
		}

		[Test]
		public async Task GetCategoryByIdAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
		{
			// Arrange
			var categoryId = Guid.NewGuid();
			var emptyCategoryList = new List<Category>().AsQueryable();

			_categoryRepositoryMock.Setup(repo => repo.FindByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
				.Returns(new TestAsyncEnumerable<Category>(emptyCategoryList));

			// Act
			var result = await _categoryService.GetCategoryByIdAsync(categoryId);

			// Assert
			result.Should().BeNull();
		}

		[Test]
		public async Task GetCategoriesAsync_ShouldReturnPaginatedList_WhenCalledWithPaging()
		{
			// Arrange
			var pageNumber = 1;
			var pageSize = 2;
			var categories = new List<Category>
			{
				new Category { Id = Guid.NewGuid(), Name = "Category1" },
				new Category { Id = Guid.NewGuid(), Name = "Category2" }
			}.AsQueryable();

			_categoryRepositoryMock.Setup(repo => repo.FindAll())
				.Returns(new TestAsyncEnumerable<Category>(categories));

			var paginatedList = await PaginatedList<CategoryDto, Category>.CreateAsync(
				categories.AsNoTracking(), pageNumber, pageSize, _mapperMock.Object);


			// Act
			var result = await _categoryService.GetCategoriesAsync(pageNumber, pageSize, null, true);

			// Assert
			result.Should().BeEquivalentTo(paginatedList);
		}

		[Test]
		public async Task GetCategoriesAsync_ShouldReturnAll_WhenCalledWithoutPaging()
		{
			// Arrange
			var pageNumber = 1;
			var pageSize = 10;
			var categories = new List<Category>
			{
				new Category { Id = Guid.NewGuid(), Name = "Category1" },
				new Category { Id = Guid.NewGuid(), Name = "Category2" }
			}.AsQueryable();

			_categoryRepositoryMock.Setup(repo => repo.FindAll())
				.Returns(new TestAsyncEnumerable<Category>(categories));

			var paginatedList = await PaginatedList<CategoryDto, Category>.CreateAsync(
				categories.AsNoTracking(), pageNumber, categories.Count(), _mapperMock.Object);

			// Act
			var result = await _categoryService.GetCategoriesAsync(pageNumber, pageSize, null, false);

			// Assert
			result.Should().BeEquivalentTo(paginatedList);
		}
	}
}

