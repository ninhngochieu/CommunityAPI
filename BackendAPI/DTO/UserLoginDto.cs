namespace BackendAPI.DTO
{
    public class UserLoginDto
    {

        public string Username { get;  set; }
        public string Token { get; set; }
        
        public string PhotoUrl { get; set; }
        public string Gender { get; set; }
        public string KnownAs { get; set; }
    }
}