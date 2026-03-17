namespace Domain.Entities;

public partial class Movimiento
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public string TypeMove { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public virtual Product Product { get; set; } = null!;
}
