using FluentValidation;
using UserManagement.DTOs;

namespace UserManagement.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.EmailOrUsername)
            .NotEmpty().WithMessage("Email or username is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");

        RuleFor(x => x.ProjectId)
            .MaximumLength(100).WithMessage("Project ID cannot exceed 100 characters");
    }
}

