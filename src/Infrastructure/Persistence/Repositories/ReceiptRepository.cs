using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ReceiptRepository : IReceiptRepository
{
    private readonly AppDbContext _db;

    public ReceiptRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(Receipt receipt, CancellationToken ct = default)
    {
        _db.Receipts.Add(receipt);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<Receipt>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Receipts
            .OrderByDescending(r => r.PurchaseDate)
            .ThenByDescending(r => r.CreatedUtc)
            .ToListAsync(ct);
    }
}