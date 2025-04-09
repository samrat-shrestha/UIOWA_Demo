using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Receipts;

public class GetAllReceiptsHandler : IRequestHandler<GetAllReceiptsQuery, List<Receipt>>
{
    private readonly IReceiptRepository _repo;

    public GetAllReceiptsHandler(IReceiptRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<Receipt>> Handle(GetAllReceiptsQuery request, CancellationToken ct)
    {
        return await _repo.GetAllAsync(ct);
    }
} 