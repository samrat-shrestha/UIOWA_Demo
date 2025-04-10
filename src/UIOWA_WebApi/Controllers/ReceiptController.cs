using Application.Receipts;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using AutoMapper;
using Application.DTOs;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceiptsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _environment;
    private readonly IMapper _mapper;

    public ReceiptsController(IMediator mediator, IWebHostEnvironment environment, IMapper mapper)
    {
        _mediator = mediator;
        _environment = environment;
        _mapper = mapper;
    }

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
        
        var dtos = _mapper.Map<List<ReceiptDto>>(receipts);
        return Ok(dtos);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReceiptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetReceiptByIdQuery(id);
        var receipt = await _mediator.Send(query, ct);
        
        if (receipt == null)
            return NotFound();
            
        return Ok(_mapper.Map<ReceiptDto>(receipt));
    }
    
    [HttpGet("{id}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadReceipt(Guid id, CancellationToken ct)
    {
        var query = new GetReceiptByIdQuery(id);
        var receipt = await _mediator.Send(query, ct);
        
        if (receipt == null)
            return NotFound();
            
        var filePath = Path.Combine(_environment.ContentRootPath, receipt.ReceiptPath);
        
        if (!System.IO.File.Exists(filePath))
            return NotFound("Receipt file not found");
            
        var fileExtension = Path.GetExtension(filePath);
        var contentType = GetContentType(fileExtension);
        
        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath, ct);
        return File(fileBytes, contentType, $"receipt-{id}{fileExtension}");
    }
    
    private string GetContentType(string fileExtension)
    {
        return fileExtension.ToLower() switch
        {
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };
    }
}