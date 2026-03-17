namespace Domain.Entities;

public partial class Rol
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool Status { get; set; }

    public DateTime CreateAt { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
