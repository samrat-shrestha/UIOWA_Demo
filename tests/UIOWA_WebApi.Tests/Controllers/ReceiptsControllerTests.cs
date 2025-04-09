using Application.Receipts;
using Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using Xunit;

namespace UIOWA_WebApi.Tests.Controllers;

public class ReceiptsControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly ReceiptsController _controller;

    public ReceiptsControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new ReceiptsController(_mockMediator.Object);
    }

    [Fact]
    public async Task Submit_ValidRequest_ReturnsCreatedResult()
    {
        // Arrange
        var purchaseDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var amount = 123.45m;
        var description = "Test Receipt";
        var mockFile = new Mock<IFormFile>();

        var receiptId = Guid.NewGuid();
        
        _mockMediator
            .Setup(m => m.Send(It.IsAny<SubmitReceiptCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(receiptId);

        var dto = new SubmitReceiptDto
        {
            PurchaseDate = purchaseDate,
            Amount = amount,
            Description = description,
            ReceiptFile = mockFile.Object
        };

        // Act
        var result = await _controller.Submit(dto, CancellationToken.None);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(ReceiptsController.Submit));
        createdResult.RouteValues.Should().ContainKey("id");
        createdResult.RouteValues!["id"].Should().Be(receiptId);
        createdResult.Value.Should().Be(receiptId);
        
        _mockMediator.Verify(m => m.Send(
            It.Is<SubmitReceiptCommand>(cmd => 
                cmd.PurchaseDate == purchaseDate && 
                cmd.Amount == amount && 
                cmd.Description == description && 
                cmd.ReceiptFile == mockFile.Object),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAll_ReturnsAllReceipts()
    {
        // Arrange
        var receipts = new List<Receipt>
        {
            new Receipt(
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                100m,
                "Receipt 1",
                "path/to/receipt1.pdf"
            ),
            new Receipt(
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)),
                200m,
                "Receipt 2",
                "path/to/receipt2.pdf"
            )
        };
        
        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllReceiptsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(receipts);

        // Act
        var result = await _controller.GetAll(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedDtos = okResult.Value.Should().BeAssignableTo<IEnumerable<object>>().Subject;
        returnedDtos.Should().HaveCount(2);
        
        _mockMediator.Verify(m => m.Send(
            It.IsAny<GetAllReceiptsQuery>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
} 