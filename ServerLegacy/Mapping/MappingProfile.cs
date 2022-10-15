using AutoMapper;
using Netrunner.Shared.Internal.Auth;
using Netrunner.Shared.Users;
using ApplicationUser = Netrunner.Shared.Internal.Auth.ApplicationUser;

namespace Netrunner.ServerLegacy.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserProfile>();
        }
    }
}