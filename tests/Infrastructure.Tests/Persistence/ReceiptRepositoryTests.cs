using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Infrastructure.Tests.Persistence;

public class ReceiptRepositoryTests
{
    [Fact]
    public async Task AddAsync_ValidReceipt_SavesToDatabase()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var repository = new ReceiptRepository(dbContext);
        
        var receipt = new Receipt(
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            123.45m,
            "Test Receipt",
            "path/to/receipt.pdf"
        );

        // Act
        await repository.AddAsync(receipt);

        // Assert
        var savedReceipt = await dbContext.Receipts.FirstOrDefaultAsync(r => r.Id == receipt.Id);
        savedReceipt.Should().NotBeNull();
        savedReceipt!.PurchaseDate.Should().Be(receipt.PurchaseDate);
        savedReceipt.Amount.Should().Be(receipt.Amount);
        savedReceipt.Description.Should().Be(receipt.Description);
        savedReceipt.ReceiptPath.Should().Be(receipt.ReceiptPath);
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleReceipts_ReturnsAllReceiptsOrderedByDateDescending()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var repository = new ReceiptRepository(dbContext);
        
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var yesterday = today.AddDays(-1);
        var twoDaysAgo = today.AddDays(-2);
        
        var receipt1 = new Receipt(yesterday, 100m, "Receipt 1", "path1.pdf");
        var receipt2 = new Receipt(today, 200m, "Receipt 2", "path2.pdf");
        var receipt3 = new Receipt(twoDaysAgo, 300m, "Receipt 3", "path3.pdf");
        
        await dbContext.Receipts.AddRangeAsync(receipt1, receipt2, receipt3);
        await dbContext.SaveChangesAsync();

        // Act
        var receipts = await repository.GetAllAsync();

        // Assert
        receipts.Should().HaveCount(3);
        receipts[0].PurchaseDate.Should().Be(today);
        receipts[1].PurchaseDate.Should().Be(yesterday);
        receipts[2].PurchaseDate.Should().Be(twoDaysAgo);
    }

    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        return new AppDbContext(options);
    }
} 