using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BackendAPI.DTO;
using BackendAPI.Extentions;
using BackendAPI.Models;
using BackendAPI.Modules;
using BackendAPI.Repository.Interface;
using BackendAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers
{

    public class UserController : BaseController
    {
        private readonly CommunityContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IPhotoService _photoService;

        public UserController(CommunityContext context, ITokenService tokenService, IMapper mapper, IUserRepository userRepository, IPhotoService photoService)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
            _userRepository = userRepository;
            _photoService = photoService;
        }

        // GET: api/User
        
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetUsers([FromQuery]UserParams @params)
        {
            var user = await _userRepository.GetUserAsync(User.GetUserName());
            //Mục đích là để loại trừ người đang lọc

            // if (user is null)
            // {
            //     return UnauthorizedResponse("Không được phép truy cập tài nguyên này");
            // }
            //Tận dụng @param để đẩy xuống // internal get set
            @params.CurrentUsername = user.UserName;
            
            var users = await _userRepository.GetUsersAsync(@params);
         
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            
            return OkResponse(users);
        }
        
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult> GetUser(string username)
        {
            var user = await _userRepository.GetUserDtoAsync(username);
            return user is null ? NotFoundResponse("Không tìm thấy user này") : OkResponse(user);
        }
        
        [HttpPost("Login")]
        public async Task<ActionResult> DoLogin(LoginDto loginDTO)
        {
            AppUser user
                = await _context.AppUsers.Include(p=>p.Photos).SingleOrDefaultAsync(x => x.UserName == loginDTO.Username);
            if (user is null) return UnauthorizedResponse("Tai khoan khong hop le");

            // using var hmac = new HMACSHA512(user.PasswordSalt);

            // var computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            // if (computerHash.Where((item, index) => item != user.PasswordHash[index]).Any())
            // {
            //     return UnauthorizedResponse("Mật khẩu không hợp lệ");
            // }
            return OkResponse(new UserLoginDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
                Gender = user.Gender,
                KnownAs = user.KnownAs
            });

        }// POST: api/User
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Register")]
        public async Task<ActionResult> PostAppUser(RegisterDto register)
        {

            if(await UserExists(register.Username))
            {
                return BadRequestResponse("Username đã tồn tại");
            }

            var user = _mapper.Map<AppUser>(register);
            
            // using var hmac = new HMACSHA512();

            user.UserName = register.Username;
            // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password));
            // user.PasswordSalt = hmac.Key;
            
            _context.Add(user);
            
            await _context.SaveChangesAsync();
            
            return OkResponse(new UserLoginDto()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = "",
                Gender =user.Gender,
                KnownAs = user.KnownAs
            });
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.AppUsers.AnyAsync(u => u.UserName == username.ToLower());
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUserName();
            var user = await _userRepository.GetUserAsync(username);
            
            _mapper.Map(memberUpdateDto, user);

            _userRepository.UpdateUser(user);
            
            if (await _userRepository.SaveAllAsync())
            {
                return OkResponse(_mapper.Map<MemberDto>(user));
            }

            return BadRequestResponse("Xảy ra lỗi khi update");
        }

        [Authorize]
        [HttpPost("Add-Photos")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserAsync(User.GetUserName());
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequestResponse(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync())
            {
                // return OkResponse(_mapper.Map<PhotoDto>(photo));
                return CreatedAtRoute("GetUser", new {username = user.UserName},_mapper.Map<PhotoDto>(photo));
            }
            
            return BadRequestResponse("Có lỗi khi thêm ảnh");
        }

        [HttpPut("Set-Main-Photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserAsync(User.GetUserName());
            var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);

            if (photo is null)
            {
                return NotFoundResponse("Không tìm thấy ảnh");
            }

            if (photo.IsMain)
            {
                return BadRequestResponse("Ảnh đã là ảnh mặc định");
            }

            var currentPhotoMain = user.Photos.FirstOrDefault(x => x.IsMain);

            if (currentPhotoMain is not null)
            {
                currentPhotoMain.IsMain = false;
            }

            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync()) return OkResponse("Đặt ảnh mặc định thành công");

            return BadRequestResponse("Có lỗi khi đặt ảnh mặc định");
        }

        [HttpDelete("Delete-Photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserAsync(User.GetUserName());

            var photo =  user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo is null)
            {
                return NotFoundResponse("Không tìm thấy ảnh");
            }

            if (photo.IsMain)
            {
                return BadRequestResponse("Bạn không thể xoá ảnh chính");
            }

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                if (result.Error is not null)
                {
                    return BadRequestResponse(result.Error.Message);
                }
            }

            user.Photos.Remove(photo);

            if (await _userRepository.SaveAllAsync())
            {
                return OkResponse("Xoá ảnh thành công");
            }

            return BadRequestResponse("Xoá ảnh thất bại. Có lỗi xảy ra");
        }
    }
}
