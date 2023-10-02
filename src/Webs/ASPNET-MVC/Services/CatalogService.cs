using System.Web;
using ASPNET_MVC.Constants;
using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models;
using ASPNET_MVC.Models.Catalog;
using Cart.API.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ASPNET_MVC.Services
{
    public class CatalogService : BaseApiService, ICatalogService
    {
        private readonly string _catalogServiceBaseUrl;

        public CatalogService(IHttpService httpService, IConfiguration configuration) : base(httpService)
        {
            Client = httpService.GetClient(ApiServiceNames.CatalogApi);
            _catalogServiceBaseUrl = configuration.GetValue<string>("ApiServiceBaseUrls:CatalogApi");
        }

        public async Task<ResponseDto> AddProductImages(ProductImagesAddRequest productImagesAddRequest)
        {
            var request = new RequestDto {
                HttpMethod = HttpMethod.Post,
                Data = productImagesAddRequest,
                Url = "/api/Product/AddImages",
                IncludeAccessToken = true,
                ContentType = ContentTypes.MultipartFormData
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }

        public async Task<ResponseDto> CreateBrand(string brandName)
        {
            var request = new RequestDto {
                HttpMethod = HttpMethod.Post,
                Data = brandName,
                Url = "/api/Brand",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }

        public async Task<ResponseDto> CreateCategory(string categoryName)
        {
            var request = new RequestDto {
                HttpMethod = HttpMethod.Post,
                Data = categoryName,
                Url = "/api/Category",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }

        public async Task<ResponseDto<ProductCreateResponse>> CreateProduct(ProductCreateRequest productCreateRequest)
        {
            var request = new RequestDto {
                HttpMethod = HttpMethod.Post,
                Data = productCreateRequest,
                Url = "/api/Product",
                IncludeAccessToken = true,
                ContentType = ContentTypes.MultipartFormData
            };

            ResponseDto response = await SendAsync(request);
            return response.ToResponseDtoWithCastedResult<ProductCreateResponse>();
        }

        public async Task<ResponseDto> DeleteProduct(string productId)
        {
            var request = new RequestDto {
                HttpMethod = HttpMethod.Delete,
                Data = null,
                Url = $"/api/Product/{productId}",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }

        public async Task<ResponseDto> DeleteProductImages(ProductImagesDeleteRequest productImagesDeleteRequest)
        {
            var request = new RequestDto {
                HttpMethod = HttpMethod.Post,
                Data = productImagesDeleteRequest,
                Url = $"/api/Product/RemoveImages",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }

        public async Task<ResponseDto<IEnumerable<BrandDto>>> GetBrandList()
        {
            var request = new RequestDto {
                HttpMethod = HttpMethod.Get,
                Data = null,
                Url = "/api/Brand",
                IncludeAccessToken = false
            };

            ResponseDto response = await SendAsync(request);
            return response.ToResponseDtoWithCastedResult<IEnumerable<BrandDto>>();
        }

        public async Task<ResponseDto<IEnumerable<CategoryDto>>> GetCategoryList()
        {
            var request = new RequestDto {
                HttpMethod = HttpMethod.Get,
                Data = null,
                Url = "/api/Category",
                IncludeAccessToken = false
            };

            ResponseDto response = await SendAsync(request);
            return response.ToResponseDtoWithCastedResult<IEnumerable<CategoryDto>>();
        }

        public async Task<ResponseDto<ProductDto>> GetProductDetails(string id)
        {
            var request = new RequestDto {
                HttpMethod = HttpMethod.Get,
                Data = null,
                Url = $"/api/Product/details/{id}",
                IncludeAccessToken = false
            };

            ResponseDto response = await SendAsync(request);
            var castedResponse = response.ToResponseDtoWithCastedResult<ProductDto>();


            if (castedResponse.Result != null && castedResponse.Result.ProductImages != null)
            {
                castedResponse.Result.ThumbnailUrl = _catalogServiceBaseUrl + "/productimages/" + castedResponse.Result.ThumbnailFileName;

                foreach (ProductImageDto image in castedResponse.Result.ProductImages)
                {
                    image.ImageUrl = _catalogServiceBaseUrl + "/productimages/" + image.ImageFileName;
                }
            }

            return castedResponse;
        }

        public async Task<ResponseDto<IEnumerable<ProductDto>>> GetProductList(string containName = "", string category = "", string brand = "", 
                                                                                string origin = "", string orderBy = "", double min = 0, double max = 0, 
                                                                                bool showDeleted = true, int page = 1)
        {
            var uriBuilder = new UriBuilder("http://temphost:8000/api/Product");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[nameof(showDeleted)] = showDeleted.ToString();
            query[nameof(page)] = page.ToString();

            if(!string.IsNullOrEmpty(containName))
            {
                query[nameof(containName)] = containName;
            }

            if(!string.IsNullOrEmpty(category))
            {
                query[nameof(category)] = category;
            }

            if(!string.IsNullOrEmpty(brand))
            {
                query[nameof(brand)] = brand;
            }

            if(!string.IsNullOrEmpty(origin))
            {
                query[nameof(origin)] = origin;
            }

            if(!string.IsNullOrEmpty(orderBy))
            {
                query[nameof(orderBy)] = orderBy;
            }

            if(min > 0)
            {
                query[nameof(min)] = min.ToString();
            }

            if(max > 0)
            {
                query[nameof(max)] = max.ToString();
            }

            uriBuilder.Query = query.ToString();

            var request = new RequestDto {
                HttpMethod = HttpMethod.Get,
                Data = null,
                Url = uriBuilder.Uri.PathAndQuery.ToString(),
                IncludeAccessToken = false
            };

            ResponseDto response = await SendAsync(request);

            var castedResponse = response.ToResponseDtoWithCastedResult<IEnumerable<ProductDto>>();

            if(castedResponse.Result != null)
            {
                foreach(ProductDto product in castedResponse.Result)
                {
                    product.ThumbnailUrl = _catalogServiceBaseUrl + "/productimages/" + product.ThumbnailFileName;
                }
            }

            return castedResponse;
        }

        public async Task<ResponseDto> PermanentDeleteProduct(string productId)
        {
            var request = new RequestDto {
                HttpMethod = HttpMethod.Delete,
                Data = null,
                Url = $"/api/Product/permanentdelete/{productId}",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }

        public Task<ResponseDto> UpdateBrandName(int brandId, string newBrandName)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> UpdateCategoryName(int categoryId, string newCategoryName)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto> UpdateProduct(ProductUpdateRequest productUpdateRequest)
        {
            var request = new RequestDto {
                HttpMethod = HttpMethod.Put,
                Data = productUpdateRequest,
                Url = $"/api/Product",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }
    }
}