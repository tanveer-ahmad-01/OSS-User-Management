using FluentValidation;
using UserManagement.DTOs;

namespace UserManagement.Validators;

public class UpdateModuleRequestValidator : AbstractValidator<UpdateModuleRequest>
{
    public UpdateModuleRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Module name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$").WithMessage("Module name can only contain letters, numbers, spaces, hyphens, and underscores")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Order must be non-negative");
    }
}

