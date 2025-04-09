using Application.Interfaces;
using Application.Receipts;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Application.Tests.Receipts;

public class SubmitReceiptHandlerTests
{
    private readonly Mock<IReceiptRepository> _mockRepository;
    private readonly Mock<IFileStore> _mockFileStore;
    private readonly SubmitReceiptHandler _handler;

    public SubmitReceiptHandlerTests()
    {
        _mockRepository = new Mock<IReceiptRepository>();
        _mockFileStore = new Mock<IFileStore>();
        _handler = new SubmitReceiptHandler(_mockRepository.Object, _mockFileStore.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_SavesFileAndCreatesReceipt()
    {
        // Arrange
        var purchaseDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var amount = 123.45m;
        var description = "Test Receipt";
        var mockFile = new Mock<IFormFile>();
        
        var command = new SubmitReceiptCommand(purchaseDate, amount, description, mockFile.Object);
        var storedPath = "path/to/receipt.pdf";
        
        _mockFileStore.Setup(fs => fs.SaveAsync(mockFile.Object, default))
            .ReturnsAsync(storedPath);

        Receipt? capturedReceipt = null;
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Receipt>(), default))
            .Callback<Receipt, CancellationToken>((r, ct) => capturedReceipt = r);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.Should().NotBe(Guid.Empty);
        _mockFileStore.Verify(fs => fs.SaveAsync(mockFile.Object, default), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Receipt>(), default), Times.Once);

        capturedReceipt.Should().NotBeNull();
        capturedReceipt!.PurchaseDate.Should().Be(purchaseDate);
        capturedReceipt.Amount.Should().Be(amount);
        capturedReceipt.Description.Should().Be(description);
        capturedReceipt.ReceiptPath.Should().Be(storedPath);
    }
} 