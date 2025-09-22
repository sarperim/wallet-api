using FluentValidation;
using Wallet.Entities.DTO;

namespace Wallet.Entities.Validators
{
    public class UserDTOValidator : AbstractValidator<UserDTO>
    { 
        public UserDTOValidator()
        {
            RuleFor(x => x.Username)
                .NotNull().WithMessage("Username cannot be null.")
                .Length(2,255).WithMessage("Username should be between 2 and 255 chars.");

            RuleFor(x => x.Password)
               .NotNull().WithMessage("Password cannot be null.")
               .Length(5, 255).WithMessage("Password should be between 5 and 255 chars.");
        }
    }
}
