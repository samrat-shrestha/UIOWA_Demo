using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Receipts;

public record SubmitReceiptCommand(
    DateOnly PurchaseDate,
    decimal Amount,
    string Description,
    IFormFile ReceiptFile) : IRequest<Guid>;