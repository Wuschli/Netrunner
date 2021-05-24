using AutoMapper;
using Netrunner.Shared.Internal.Auth;
using Netrunner.Shared.Users;

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