using NetEscapades.Configuration.Validation;
using System.ComponentModel.DataAnnotations;

namespace BudgetCast.Identity.Api.Infrastructure.AppSettings
{
    public abstract class Validatable : IValidatable
    {
        public virtual void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
        }
    }
}
