using FluentValidation;

namespace API.DAL.User.Models
{
    public class CreateUserModelValidator : AbstractValidator<CreateUserModel>
    {
        public CreateUserModelValidator()
        {
            RuleFor(x => x.UserName).NotNull().NotEmpty();
            RuleFor(x => x.Password).NotNull().NotEmpty();
            RuleFor(x => x.KnownAs).NotNull().NotEmpty();
            RuleFor(x => x.City).NotNull().NotEmpty();
            RuleFor(x => x.Country).NotNull().NotEmpty();
            RuleFor(x => x.Gender).NotNull().NotEmpty();
            RuleFor(x => x.DateOfBirth).NotNull().NotEmpty();
            RuleFor(x => x.PlanId).GreaterThan(0);
        }
    }
}
