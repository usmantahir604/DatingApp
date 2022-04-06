using FluentValidation;

namespace API.DAL.User.Models
{
    public class LoginUserModelValidator : AbstractValidator<LoginUserModel>
    {
        public LoginUserModelValidator()
        {
            RuleFor(x => x.UserName).NotNull().NotEmpty();
            RuleFor(x => x.Password).NotNull().NotEmpty();
        }
    }
}
