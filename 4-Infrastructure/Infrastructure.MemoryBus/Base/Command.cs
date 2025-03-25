using FluentValidation.Results;
using MediatR;
using Modules.Core.Shared;

namespace Infrastructure.MemoryBus.Base
{
    public abstract class Command : Message, IRequest<Result>
    {
        public DateTime Timestamp { get; private set; }
        public ValidationResult ValidationResult { get; set; }

        protected Command()
        {
            Timestamp = DateTime.Now;
            ValidationResult = new ValidationResult();
        }

        public virtual bool IsValid()
        {
            return ValidationResult.IsValid;
        }
        public virtual IEnumerable<string> ErrorMessages()
        {
            return ValidationResult.Errors.Select(s => s.ErrorMessage);
        }
    }
}