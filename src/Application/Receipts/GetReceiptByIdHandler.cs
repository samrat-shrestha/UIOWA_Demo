using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Receipts;

public class GetReceiptByIdHandler : IRequestHandler<GetReceiptByIdQuery, Receipt?>
{
    private readonly IReceiptRepository _repository;

    public GetReceiptByIdHandler(IReceiptRepository repository)
    {
        _repository = repository;
    }

    public async Task<Receipt?> Handle(GetReceiptByIdQuery request, CancellationToken ct)
    {
        return await _repository.GetByIdAsync(request.Id, ct);
    }
}
