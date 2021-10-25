using Login.Integration.Interface.Commands;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Login.Integration.Interface.Validator
{
    public class RegisterCommandValidator : ValidatorBase<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Must be valid email address");

            RuleFor(x => x.Password)
                .Must(BeValidPassword)
                .WithMessage("Password must contains at least 1 lowercase, 1 uppercase, 1 digit, and 1 special character.");

            RuleFor(x => x.Password)
                .MinimumLength(5)
                .MaximumLength(13)
                .WithMessage("Password length must between 5 and 13 characters.");


            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Passwords must match.");
        }

        private bool BeValidPassword(string password)
        {
            var lowercase = new Regex("[a-z]+");
            var uppercase = new Regex("[A-Z]+");
            var digit = new Regex("(\\d)+");
            var symbol = new Regex("(\\W)+");

            return
                lowercase.IsMatch(password) &&
                uppercase.IsMatch(password) &&
                digit.IsMatch(password) &&
                symbol.IsMatch(password);
        }

    }
}
