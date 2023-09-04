using AutoMapper;
using Discount.API.Domain.Entities;
using Discount.API.Models;
using EventBus.Events;

namespace Discount.API.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Coupon, CouponDto>().ReverseMap();
            CreateMap<Coupon, CouponDeletedEvent>();
            CreateMap<CouponCreateRequest, Coupon>();
        }
    }
}
