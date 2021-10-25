using FluentValidation;

namespace Login.Integration.Interface.Validator
{
    public class ValidatorBase<T> : AbstractValidator<T> where T : class
    {
    }
}
