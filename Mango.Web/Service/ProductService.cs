using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService baseService;

        public ProductService(IBaseService baseService)
        {
            this.baseService = baseService;
        }
        public async Task<ResponseDto?> CreateProductsAsync(ProductDto ProductDto)
        {
            return await baseService.SendAsync(new RequestDto() 
            {
                ApiType=SD.ApiType.POST,
                Data=ProductDto,
                Url=SD.ProductAPIBase+"/api/product/"
            
            });
        }

        public Task<ResponseDto?> DeleteProductsAsync(int id)
        {
            return baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.ProductAPIBase + "/api/product/" + id

            });
        }

        public async Task<ResponseDto?> GetAllProductsAsync()
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url=SD.ProductAPIBase+"/api/product",

            });
        }

        public async Task<ResponseDto?> GetProductAsync(string ProductCode)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/product/GetByCode" + ProductCode,

            });

        }

        public Task<ResponseDto?> GetProductByIdAsync(int id)
        {
            return baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/product/" + id

            });
        }

        public async Task<ResponseDto?> UpdateProductsAsync(ProductDto ProductDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = ProductDto,
                Url = SD.ProductAPIBase + "/api/product/"

            });
        }
    }
}
