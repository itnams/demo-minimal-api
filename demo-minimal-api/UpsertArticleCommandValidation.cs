using System;
using FluentValidation;

namespace demo_minimal_api
{
    public class UpsertArticleCommandValidation : AbstractValidator<UpsertArticleCommand>
    {
        public UpsertArticleCommandValidation()
        {
            RuleFor(article => article.Title)
           .NotEmpty().WithMessage("Title is required")
           .Length(1, 50).WithMessage(" must be between 1 and 50 characters");

            RuleFor(article => article.Content)
           .NotEmpty().WithMessage("Content is required");

            RuleFor(article => article.CreatedDate)
           .NotEmpty().WithMessage("Created Date is required")
           .Must(ValidDateFormat).WithMessage("Invalid date format");

            RuleFor(article => article.UpdatedDate)
           .NotEmpty().WithMessage("Updated Date is required")
           .Must(ValidDateFormat).WithMessage("Invalid date format");
        }
        private bool ValidDateFormat(string dateString)
        {
            return DateTime.TryParseExact(dateString, "yyyy-MM-ddThh:mm", null, System.Globalization.DateTimeStyles.None, out _);
        }
    }
}
