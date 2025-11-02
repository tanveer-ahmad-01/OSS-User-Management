using FluentValidation;
using UserManagement.DTOs;

namespace UserManagement.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters")
            .MaximumLength(100).WithMessage("Username cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"(?=.*[a-z])").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"(?=.*[A-Z])").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"(?=.*\d)").WithMessage("Password must contain at least one number")
            .Matches(@"(?=.*[@$!%*?&#])").WithMessage("Password must contain at least one special character");

        RuleFor(x => x.FirstName)
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.ProjectId)
            .MaximumLength(100).WithMessage("Project ID cannot exceed 100 characters");
    }
}

