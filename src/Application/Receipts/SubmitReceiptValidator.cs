using FluentValidation;

namespace Application.Receipts;

public class SubmitReceiptValidator : AbstractValidator<SubmitReceiptCommand>
{
    public SubmitReceiptValidator()
    {
        RuleFor(x => x.PurchaseDate)
            .Must(d => d <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Purchase date cannot be in the future");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .LessThanOrEqualTo(99999.99m);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.ReceiptFile)
            .NotNull()
            .Must(f => f.Length > 0 && f.Length <= 5 * 1024 * 1024)
            .WithMessage("Receipt file must be <= 5 MB")
            .Must(f => new[] { "application/pdf", "image/png", "image/jpeg" }.Contains(f.ContentType))
            .WithMessage("Allowed file types: PDF, PNG, JPEG");
    }
}