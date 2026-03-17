namespace Domain
{
    public class PaginationRequest
    {
        public int? Page { get; set; } = 1;
        public int? Take { get; set; } = 6;
        public string? Sort { get; set; } = "";
        public string[]? Filters { get; set; }
    }
}