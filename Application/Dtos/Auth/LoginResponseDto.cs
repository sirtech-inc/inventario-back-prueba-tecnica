namespace Application.Dtos.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;

        public string Fullname { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Rol { get; set; } = null!;

        public DateTime Expiration { get; set; }
    }
}
