using AutoMapper;
using Netrunner.Server.Identity.Data;
using Netrunner.Shared.User;

namespace Netrunner.Server.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserProfile>();
        }
    }
}