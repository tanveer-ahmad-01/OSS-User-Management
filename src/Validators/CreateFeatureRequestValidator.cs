using FluentValidation;
using UserManagement.DTOs;

namespace UserManagement.Validators;

public class CreateFeatureRequestValidator : AbstractValidator<CreateFeatureRequest>
{
    public CreateFeatureRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Feature name is required")
            .MaximumLength(100).WithMessage("Feature name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$").WithMessage("Feature name can only contain letters, numbers, spaces, hyphens, and underscores");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Feature code is required")
            .MaximumLength(50).WithMessage("Feature code cannot exceed 50 characters")
            .Matches(@"^[A-Z0-9_]+$").WithMessage("Feature code can only contain uppercase letters, numbers, and underscores");

        RuleFor(x => x.ModuleId)
            .NotEmpty().WithMessage("Module ID is required");

        RuleFor(x => x.ProjectId)
            .MaximumLength(100).WithMessage("Project ID cannot exceed 100 characters");
    }
}

