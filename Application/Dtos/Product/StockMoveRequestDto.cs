namespace Application.Dtos.Product
{
    public class StockMoveRequestDto
    {
        public int Quantity { get; set; }
        public string TypeMove { get; set; } = null!;
    }
}

