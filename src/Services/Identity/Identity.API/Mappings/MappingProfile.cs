using AutoMapper;
using EventBus.Events;
using Identity.API.Domain.Entities;

namespace Identity.API.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserCreatedIntegrationEvent>();
        }
    }
}