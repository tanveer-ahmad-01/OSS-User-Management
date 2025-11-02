using FluentValidation;
using UserManagement.DTOs;

namespace UserManagement.Validators;

public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required")
            .MaximumLength(100).WithMessage("Role name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$").WithMessage("Role name can only contain letters, numbers, spaces, hyphens, and underscores");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage("Priority must be non-negative");

        RuleFor(x => x.ProjectId)
            .MaximumLength(100).WithMessage("Project ID cannot exceed 100 characters");
    }
}

