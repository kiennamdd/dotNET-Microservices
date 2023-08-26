using AutoMapper;
using Catalog.API.Domain.Entities;
using Catalog.API.Models;

namespace Catalog.API.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(o => o.CategoryName, opt => opt.MapFrom(o => o.Category != null ? o.Category.Name : string.Empty))
                .ForMember(o => o.BrandName, opt => opt.MapFrom(o => o.Brand != null ? o.Brand.Name : string.Empty));

            CreateMap<ProductCreateRequest, Product>();

            CreateMap<ProductImage, ProductImageDto>();

            CreateMap<Brand, BrandDto>();

            CreateMap<Category, CategoryDto>();
        }
    }
}