using FluentValidation;

namespace Modules.Tasks.Domain.Base_sys_tasks_log.Validations
{
    public abstract class Base_sys_tasks_logValidation : AbstractValidator<Base_sys_tasks_logModel>
    {
        protected void ValidateId()
        {
            //RuleFor(c => c.ID).NotEqual("");
        }
    }
}

