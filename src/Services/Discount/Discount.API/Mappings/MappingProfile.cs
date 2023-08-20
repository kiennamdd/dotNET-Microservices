using AutoMapper;
using Discount.API.Domain.Entities;
using Discount.API.Models;

namespace Discount.API.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Coupon, CouponDto>().ReverseMap();
            CreateMap<CouponCreateRequest, Coupon>();
        }
    }
}
