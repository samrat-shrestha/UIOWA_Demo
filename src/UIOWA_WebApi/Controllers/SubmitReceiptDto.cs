using Microsoft.AspNetCore.Http;

namespace WebApi.Controllers;

public class SubmitReceiptDto
{
    public DateOnly PurchaseDate { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public IFormFile? ReceiptFile { get; set; }
}