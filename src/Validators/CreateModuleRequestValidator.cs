using FluentValidation;
using UserManagement.DTOs;

namespace UserManagement.Validators;

public class CreateModuleRequestValidator : AbstractValidator<CreateModuleRequest>
{
    public CreateModuleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Module name is required")
            .MaximumLength(100).WithMessage("Module name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$").WithMessage("Module name can only contain letters, numbers, spaces, hyphens, and underscores");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Module code is required")
            .MaximumLength(50).WithMessage("Module code cannot exceed 50 characters")
            .Matches(@"^[A-Z0-9_]+$").WithMessage("Module code can only contain uppercase letters, numbers, and underscores");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Order must be non-negative");

        RuleFor(x => x.ProjectId)
            .MaximumLength(100).WithMessage("Project ID cannot exceed 100 characters");
    }
}

