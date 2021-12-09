using System.Linq;
using AutoMapper;
using BackendAPI.DTO;
using BackendAPI.Models;

namespace BackendAPI.Modules
{
    internal class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<AppUser, UserLoginDto>();
            CreateMap<AppUser, MemberDto>()
                .ForMember(d=>d.PhotoUrl,
                    o => 
                        o.MapFrom(p=>p.Photos.FirstOrDefault(x=>x.IsMain).Url));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
        }
    }
}