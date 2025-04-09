namespace Domain.Entities;

public class Receipt
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateOnly PurchaseDate { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string ReceiptPath { get; private set; } = string.Empty;
    public DateTime CreatedUtc { get; private set; } = DateTime.UtcNow;

    private Receipt() { } // EF Core

    public Receipt(DateOnly purchaseDate, decimal amount, string description, string receiptPath)
    {
        if (purchaseDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Purchase date cannot be in the future", nameof(purchaseDate));
        if (amount <= 0 || amount > 99999.99m)
            throw new ArgumentException("Amount must be > 0 and <= 99999.99", nameof(amount));
        if (string.IsNullOrWhiteSpace(receiptPath))
            throw new ArgumentException("Receipt path cannot be empty", nameof(receiptPath));

        PurchaseDate = purchaseDate;
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Description = description.Trim();
        ReceiptPath = receiptPath;
    }
}