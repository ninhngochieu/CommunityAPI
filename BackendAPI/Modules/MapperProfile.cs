using AutoMapper;
using BackendAPI.Controllers;
using BackendAPI.Models;

namespace BackendAPI
{
    internal class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<AppUser, UserDTO>();
        }
    }
}