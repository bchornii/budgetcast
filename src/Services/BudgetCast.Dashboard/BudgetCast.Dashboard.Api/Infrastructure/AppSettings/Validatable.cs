using System.ComponentModel.DataAnnotations;
using NetEscapades.Configuration.Validation;

namespace BudgetCast.Dashboard.Api.Infrastructure.AppSettings
{
    public abstract class Validatable : IValidatable
    {
        public virtual void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
        }
    }
}