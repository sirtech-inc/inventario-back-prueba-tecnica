namespace Domain.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Fullname { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RolId { get; set; }

    public bool Status { get; set; }

    public DateTime CreateAt { get; set; }

    public virtual Rol Rol { get; set; } = null!;
}
