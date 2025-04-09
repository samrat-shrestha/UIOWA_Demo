using Domain.Entities;
using MediatR;

namespace Application.Receipts;

public record GetReceiptByIdQuery(Guid Id) : IRequest<Receipt?>;
