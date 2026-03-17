namespace Application.Dtos.User
{
    public class UserRequestDto
    {
        public string Fullname { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public int RolId { get; set; }
    }
}
