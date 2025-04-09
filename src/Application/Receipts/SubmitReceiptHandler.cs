using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Receipts;

public class SubmitReceiptHandler : IRequestHandler<SubmitReceiptCommand, Guid>
{
    private readonly IReceiptRepository _repo;
    private readonly IFileStore _fileStore;

    public SubmitReceiptHandler(IReceiptRepository repo, IFileStore fileStore)
    {
        _repo = repo;
        _fileStore = fileStore;
    }

    public async Task<Guid> Handle(SubmitReceiptCommand cmd, CancellationToken ct)
    {
        // Save file first
        var storedPath = await _fileStore.SaveAsync(cmd.ReceiptFile, ct);

        // Create domain entity
        var receipt = new Receipt(cmd.PurchaseDate, cmd.Amount, cmd.Description, storedPath);

        // Persist
        await _repo.AddAsync(receipt, ct);

        return receipt.Id;
    }
}