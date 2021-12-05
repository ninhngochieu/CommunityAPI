using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Controllers
{
    public class LoginDTO
    {

        [Required]
        public string Username { get;  set; }
        [Required]
        public string Password { get; set; }
    }
}