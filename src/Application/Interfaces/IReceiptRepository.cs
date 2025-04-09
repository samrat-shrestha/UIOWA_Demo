using Domain.Entities;

namespace Application.Interfaces;

public interface IReceiptRepository
{
    Task AddAsync(Receipt receipt, CancellationToken ct = default);
    Task<List<Receipt>> GetAllAsync(CancellationToken ct = default);
}