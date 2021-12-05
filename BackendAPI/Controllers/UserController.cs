using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendAPI.Models;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http.Description;
using System;
using Microsoft.AspNetCore.Authorization;
using AutoMapper.QueryableExtensions;
using AutoMapper;

namespace BackendAPI.Controllers
{

    public class UserController : BaseController
    {
        private readonly CommunityContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public UserController(CommunityContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        // GET: api/User

        [HttpGet]
        public async Task<ActionResult> GetUsers()
        {
            UserDTO[] userDTOs = await _context.AppUsers.ProjectTo<UserDTO>(_mapper.ConfigurationProvider).ToArrayAsync();
            return OkResponse(userDTOs);
        }

            [HttpPost("Login")]
        public async Task<ActionResult> DoLogin(LoginDTO loginDTO)
        {
            AppUser user
                = await _context.AppUsers.SingleOrDefaultAsync(x => x.UserName == loginDTO.Username);
            if (user is null) return UnauthorizedResponse("Tai khoan khong hop le");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            byte[] computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for (int i = 0; i < computerHash.Length; i++)
            {
                if (computerHash[i] != user.PasswordHash[i]) return UnauthorizedResponse("Mật khẩu không hợp lệ");
            }

            return OkResponse(new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            });

        }// POST: api/User
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Register")]
        public async Task<ActionResult> PostAppUser(RegisterDTO register)
        {
            if(await UserExists(register.Username))
            {
                return BadRequestResponse("Username đã tồn tại");
            }

            using var hmac = new HMACSHA512();
            AppUser user = new AppUser
            {
                UserName = register.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Add(user);
            await _context.SaveChangesAsync();
            return OkResponse(user);
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.AppUsers.AnyAsync(u => u.UserName == username.ToLower());
        }
    }
}
