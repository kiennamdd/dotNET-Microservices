using AutoMapper;
using Catalog.API.Domain.Entities;
using Catalog.API.Interfaces;
using Catalog.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;    
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ResponseDto> Get()
        {
            IEnumerable<Category> list = await _categoryRepository.GetListAsync();

            return ResponseDto.Success(result: _mapper.Map<IEnumerable<CategoryDto>>(list));
        }

        [HttpPost]
        public async Task<ResponseDto> Post([FromBody] string categoryName)
        {
            categoryName = categoryName.Trim();
            if(string.IsNullOrEmpty(categoryName))
            {
                return ResponseDto.Fail("'CategoryName' is required.");
            }

            if(categoryName.Length > 50)
            {
                return ResponseDto.Fail("'CategoryName' must be less than 50 characters.");
            }

            Category? existedCategory = await _categoryRepository.GetByNameAsync(categoryName);
            if(existedCategory != null)
            {
                return ResponseDto.Fail($"Category '{categoryName}' already exists.");
            }

            _categoryRepository.Add(new Category { Name = categoryName });
            await _unitOfWork.SaveChangesAsync();

            return ResponseDto.Success($"Category '{categoryName}' has been created.");
        }

        [HttpPut]
        public async Task<ResponseDto> Put([FromBody] int id, string categoryName)
        {
            categoryName = categoryName.Trim();
            if(string.IsNullOrEmpty(categoryName))
            {
                return ResponseDto.Fail("'CategoryName' is required.");
            }

            if(categoryName.Length > 50)
            {
                return ResponseDto.Fail("'CategoryName' must be less than 50 characters.");
            }

            Category? existedCategoryById = await _categoryRepository.GetByIdAsync(id);
            if(existedCategoryById is null)
            {
                return ResponseDto.Fail("Not found.");
            }

            Category? existedCategoryByName = await _categoryRepository.GetByNameAsync(categoryName);
            if(existedCategoryByName != null)
            {
                if(existedCategoryByName.Id != existedCategoryById.Id)
                {
                    return ResponseDto.Fail($"Category '{categoryName}' already exists.");
                }

                return ResponseDto.Success($"Category '{categoryName}' has been updated.");
            }

            existedCategoryById.Name = categoryName;

            _categoryRepository.Update(existedCategoryById);
            await _unitOfWork.SaveChangesAsync();

            return ResponseDto.Success($"Category updated successfully.");
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ResponseDto> Delete(int id)
        {
            Category? existedCategory = await _categoryRepository.GetByIdAsync(id);
            if(existedCategory is null)
            {
                return ResponseDto.Fail("Not found.");
            }

            _categoryRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();

            return ResponseDto.Success($"Category with identifier {id} has been deleted.");
        }
    }
}