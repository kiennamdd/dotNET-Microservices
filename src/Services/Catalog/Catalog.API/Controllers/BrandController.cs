using AutoMapper;
using Catalog.API.Domain.Entities;
using Catalog.API.Interfaces;
using Catalog.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BrandController(IBrandRepository brandRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _brandRepository = brandRepository;    
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ResponseDto> Get()
        {
            IEnumerable<Brand> list = await _brandRepository.GetListAsync();

            return ResponseDto.Success(result: _mapper.Map<IEnumerable<BrandDto>>(list));
        }

        [HttpPost]
        public async Task<ResponseDto> Post([FromBody] string brandName)
        {
            brandName = brandName.Trim();
            if(string.IsNullOrEmpty(brandName))
            {
                return ResponseDto.Fail("'BrandName' is required.");
            }

            if(brandName.Length > 50)
            {
                return ResponseDto.Fail("'BrandName' must be less than 50 characters.");
            }

            Brand? existedBrand = await _brandRepository.GetByNameAsync(brandName);
            if(existedBrand != null)
            {
                return ResponseDto.Fail($"Brand '{brandName}' already exists.");
            }

            _brandRepository.Add(new Brand { Name = brandName });
            await _unitOfWork.SaveChangesAsync();

            return ResponseDto.Success($"Brand '{brandName}' has been created.");
        }

        [HttpPut]
        public async Task<ResponseDto> Put([FromBody] int id, string brandName)
        {
            brandName = brandName.Trim();
            if(string.IsNullOrEmpty(brandName))
            {
                return ResponseDto.Fail("'BrandName' is required.");
            }

            if(brandName.Length > 50)
            {
                return ResponseDto.Fail("'BrandName' must be less than 50 characters.");
            }

            Brand? existedBrandById = await _brandRepository.GetByIdAsync(id);
            if(existedBrandById is null)
            {
                return ResponseDto.Fail("Not found.");
            }

            Brand? existedBrandByName = await _brandRepository.GetByNameAsync(brandName);
            if(existedBrandByName != null)
            {
                if(existedBrandByName.Id != existedBrandById.Id)
                {
                    return ResponseDto.Fail($"Brand '{brandName}' already exists.");
                }

                return ResponseDto.Success($"Brand '{brandName}' has been updated.");
            }

            existedBrandById.Name = brandName;

            _brandRepository.Update(existedBrandById);
            await _unitOfWork.SaveChangesAsync();

            return ResponseDto.Success($"Brand updated successfully.");
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ResponseDto> Delete(int id)
        {
            Brand? existedBrand = await _brandRepository.GetByIdAsync(id);
            if(existedBrand is null)
            {
                return ResponseDto.Fail("Not found.");
            }

            

            _brandRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();

            return ResponseDto.Success($"Brand with identifier {id} has been deleted.");
        }
    }
}