using ASPNET_MVC.Models.Catalog;
using Cart.API.Models;

namespace ASPNET_MVC.Interfaces
{
    public interface ICatalogService: IBaseApiService
    {
        Task<ResponseDto<IEnumerable<ProductDto>>> GetProductList(string containName = "", string category = "", string brand = "", 
                                                                    string origin = "", string orderBy = "", double min = 0, double max = 0, 
                                                                    bool showDeleted = true, int page = 1);
        Task<ResponseDto<ProductDto>> GetProductDetails(string id);
        Task<ResponseDto<ProductCreateResponse>> CreateProduct(ProductCreateRequest productCreateRequest);
        Task<ResponseDto> DeleteProduct(string productId);
        Task<ResponseDto> PermanentDeleteProduct(string productId);
        Task<ResponseDto> UpdateProduct(ProductUpdateRequest productUpdateRequest);
        Task<ResponseDto> DeleteProductImages(ProductImagesDeleteRequest productImagesDeleteRequest);
        Task<ResponseDto> AddProductImages(ProductImagesAddRequest productImagesAddRequest);
        Task<ResponseDto<IEnumerable<BrandDto>>> GetBrandList();
        Task<ResponseDto> CreateBrand(string brandName);
        Task<ResponseDto> UpdateBrandName(int brandId, string newBrandName);
        Task<ResponseDto<IEnumerable<CategoryDto>>> GetCategoryList();
        Task<ResponseDto> CreateCategory(string categoryName);
        Task<ResponseDto> UpdateCategoryName(int categoryId, string newCategoryName);
    }
}