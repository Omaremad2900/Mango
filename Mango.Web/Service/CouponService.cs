using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService baseService;

        public CouponService(IBaseService baseService)
        {
            this.baseService = baseService;
        }
        public async Task<ResponseDto?> CreateCouponsAsync(CouponDto couponDto)
        {
            return await baseService.SendAsync(new RequestDto() 
            {
                ApiType=SD.ApiType.POST,
                Data=couponDto,
                Url=SD.CouponAPIBase+"/api/coupon/"
            
            });
        }

        public Task<ResponseDto?> DeleteCouponsAsync(int id)
        {
            return baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.CouponAPIBase + "/api/coupon/" + id

            });
        }

        public async Task<ResponseDto?> GetAllCouponsAsync()
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url=SD.CouponAPIBase+"/api/coupon",

            });
        }

        public async Task<ResponseDto?> GetCouponAsync(string couponCode)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon/GetByCode" + couponCode,

            });

        }

        public Task<ResponseDto?> GetCouponByIdAsync(int id)
        {
            return baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon/" + id

            });
        }

        public async Task<ResponseDto?> UpdateCouponsAsync(CouponDto couponDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = couponDto,
                Url = SD.CouponAPIBase + "/api/coupn/"

            });
        }
    }
}
