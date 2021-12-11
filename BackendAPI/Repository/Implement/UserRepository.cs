using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackendAPI.Controllers;
using BackendAPI.DTO;
using BackendAPI.Models;
using BackendAPI.Modules;
using BackendAPI.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

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

        public async Task<PagedList<MemberDto>> GetUsersAsync(UserParams @params)
        {
            var queryable = _context.AppUsers.Include(p=>p.Photos).AsQueryable();

            queryable = queryable.Where(u => u.UserName != @params.CurrentUsername);

            queryable = queryable.Where(u => u.Gender == @params.Gender);

            if (@params.MinAge > @params.MaxAge) //Tối ưu hoá để không query nếu năm không hợp lệ
            {
                return await PagedList<MemberDto>.CreateNullListAsync();
            }

            if (@params.MaxAge>0) // Kiểm tra điều kiện mới cho tính năm
            {
                var minDate = DateTime.Today.AddYears(-@params.MaxAge - 1); // Tìm tuổi qua ddmmyyyy cho nhanh
                queryable = queryable.Where(u => u.DateOfBirth >= minDate);   
            }

            if (@params.MinAge>0)
            {
                var maxDate = DateTime.Today.AddYears(-@params.MinAge);
                queryable = queryable.Where(u=>u.DateOfBirth <= maxDate);    
            }

            return await PagedList<MemberDto>.CreateAsync(queryable
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsNoTracking(), @params.PageNumber, @params.PageSize);
        }

        public async Task<MemberDto> GetUserDtoAsync(string username)
        {
            return  _mapper.Map<MemberDto>( await GetUserAsync(username));
        }
        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() != 0;
        }

        public async Task<AppUser> GetUserAsync(string username)
        {
            return await _context.AppUsers.Include(p=>p.Photos).SingleOrDefaultAsync(x => x.UserName.ToLower() == username);
        }

        public void UpdateUser(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<AppUser> GetUserByIdAsync(int userId)
        {
            return await _context.AppUsers.FindAsync(userId);
        }
    }
}