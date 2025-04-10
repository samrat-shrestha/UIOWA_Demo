namespace Application.DTOs
{
    public class ReceiptDto
    {
        public Guid Id { get; set; }
        public DateOnly PurchaseDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ReceiptFileName { get; set; } = string.Empty;
        public DateTime CreatedUtc { get; set; }
    }
} 