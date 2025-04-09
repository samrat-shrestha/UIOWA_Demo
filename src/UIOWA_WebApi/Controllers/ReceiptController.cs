using Application.Receipts;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceiptsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReceiptsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> Submit([FromForm] SubmitReceiptDto dto, CancellationToken ct)
    {
        var cmd = new SubmitReceiptCommand(dto.PurchaseDate, dto.Amount, dto.Description ?? string.Empty, dto.ReceiptFile!);
        var id = await _mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(Submit), new { id }, id);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ReceiptDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var query = new GetAllReceiptsQuery();
        var receipts = await _mediator.Send(query, ct);
        
        var dtos = receipts.Select(MapToDto).ToList();
        return Ok(dtos);
    }

    private static ReceiptDto MapToDto(Receipt receipt) => new()
    {
        Id = receipt.Id,
        PurchaseDate = receipt.PurchaseDate,
        Amount = receipt.Amount,
        Description = receipt.Description,
        ReceiptPath = receipt.ReceiptPath,
        CreatedUtc = receipt.CreatedUtc
    };
}