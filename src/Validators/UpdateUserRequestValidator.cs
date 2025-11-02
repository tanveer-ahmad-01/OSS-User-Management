using FluentValidation;
using UserManagement.DTOs;
using UserManagement.Models;

namespace UserManagement.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Avatar)
            .MaximumLength(500).WithMessage("Avatar URL cannot exceed 500 characters");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid user status");
    }
}

