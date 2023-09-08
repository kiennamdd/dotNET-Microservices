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
            CreateMap<Coupon, CouponDeletedEvent>()
                .ForMember(o => o.CouponId, o => o.MapFrom(o => o.Id));
            CreateMap<CouponCreateRequest, Coupon>();
        }
    }
}
