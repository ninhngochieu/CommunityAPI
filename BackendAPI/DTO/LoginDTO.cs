using System.ComponentModel.DataAnnotations;

namespace BackendAPI.DTO
{
    public class LoginDto
    {

        [Required(ErrorMessage ="Tên của username không hợp lệ")]
        public string Username { get;  set; }
        [Required(ErrorMessage ="Mật khẩu không hợp lệ")]
        public string Password { get; set; }
    }
}