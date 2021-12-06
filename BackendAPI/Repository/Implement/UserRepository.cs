using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackendAPI.Controllers;
using BackendAPI.DTO;
using BackendAPI.Models;
using BackendAPI.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Repository.Implement
{
    internal class UserRepository: IUserRepository
    {
        private readonly CommunityContext _context;
        private readonly IMapper _mapper;

        public UserRepository(CommunityContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<MemberDto[]> GetUsersAsync()
        {
            return _context.AppUsers.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).ToArrayAsync();
        }

        public async Task<MemberDto> GetUsernameAsync(string username)
        {
            return _mapper.Map<MemberDto>(await _context.AppUsers
                .Include(p=>p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username));
        }
    }
}