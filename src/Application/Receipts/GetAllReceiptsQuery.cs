using Domain.Entities;
using MediatR;

namespace Application.Receipts;

public record GetAllReceiptsQuery() : IRequest<List<Receipt>>; 