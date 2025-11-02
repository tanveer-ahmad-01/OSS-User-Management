using FluentValidation;
using UserManagement.DTOs;

namespace UserManagement.Validators;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(8).WithMessage("New password must be at least 8 characters")
            .Matches(@"(?=.*[a-z])").WithMessage("New password must contain at least one lowercase letter")
            .Matches(@"(?=.*[A-Z])").WithMessage("New password must contain at least one uppercase letter")
            .Matches(@"(?=.*\d)").WithMessage("New password must contain at least one number")
            .Matches(@"(?=.*[@$!%*?&#])").WithMessage("New password must contain at least one special character");

        RuleFor(x => x.CurrentPassword)
            .NotEqual(x => x.NewPassword).WithMessage("New password must be different from current password");
    }
}

