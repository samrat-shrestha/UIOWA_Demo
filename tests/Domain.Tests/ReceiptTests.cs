using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Domain.Tests;

public class ReceiptTests
{
    [Fact]
    public void Receipt_WithValidParameters_CreatesReceipt()
    {
        // Arrange
        var purchaseDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        decimal amount = 123.45m;
        string description = "Test Receipt";
        string receiptPath = "path/to/receipt.pdf";

        // Act
        var receipt = new Receipt(purchaseDate, amount, description, receiptPath);

        // Assert
        receipt.Id.Should().NotBe(Guid.Empty);
        receipt.PurchaseDate.Should().Be(purchaseDate);
        receipt.Amount.Should().Be(amount);
        receipt.Description.Should().Be(description);
        receipt.ReceiptPath.Should().Be(receiptPath);
        receipt.CreatedUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void Receipt_WithFuturePurchaseDate_ThrowsArgumentException()
    {
        // Arrange
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        decimal amount = 123.45m;
        string description = "Test Receipt";
        string receiptPath = "path/to/receipt.pdf";

        // Act & Assert
        Action act = () => new Receipt(futureDate, amount, description, receiptPath);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Purchase date cannot be in the future*")
            .WithParameterName("purchaseDate");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100000)]
    public void Receipt_WithInvalidAmount_ThrowsArgumentException(decimal invalidAmount)
    {
        // Arrange
        var purchaseDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        string description = "Test Receipt";
        string receiptPath = "path/to/receipt.pdf";

        // Act & Assert
        Action act = () => new Receipt(purchaseDate, invalidAmount, description, receiptPath);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Amount must be > 0 and <= 99999.99*")
            .WithParameterName("amount");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Receipt_WithEmptyReceiptPath_ThrowsArgumentException(string invalidPath)
    {
        // Arrange
        var purchaseDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        decimal amount = 123.45m;
        string description = "Test Receipt";

        // Act & Assert
        Action act = () => new Receipt(purchaseDate, amount, description, invalidPath);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Receipt path cannot be empty*")
            .WithParameterName("receiptPath");
    }

    [Fact]
    public void Receipt_WithNullReceiptPath_ThrowsArgumentException()
    {
        // Arrange
        var purchaseDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        decimal amount = 123.45m;
        string description = "Test Receipt";
        string? invalidPath = null;

        // Act & Assert
        Action act = () => new Receipt(purchaseDate, amount, description, invalidPath!);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Receipt path cannot be empty*")
            .WithParameterName("receiptPath");
    }

    [Fact]
    public void Receipt_WithWhitespaceDescription_TrimsDescription()
    {
        // Arrange
        var purchaseDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        decimal amount = 123.45m;
        string description = "  Test Receipt with spaces   ";
        string receiptPath = "path/to/receipt.pdf";

        // Act
        var receipt = new Receipt(purchaseDate, amount, description, receiptPath);

        // Assert
        receipt.Description.Should().Be("Test Receipt with spaces");
    }
} 