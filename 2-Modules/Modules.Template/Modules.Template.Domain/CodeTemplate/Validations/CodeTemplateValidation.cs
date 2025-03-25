using FluentValidation;
using Modules.Core.Domain.Enums;

namespace Modules.Template.Domain.CodeTemplate.Validations
{
    public abstract class CodeTemplateValidation : AbstractValidator<CodeTemplateModel>
    {
        public CodeTemplateValidation()
        {
            RuleFor(s => s.username).NotNull().WithMessage("用户名不能为空。");
            //RuleFor(s => s.DbInfo).NotNull().WithMessage("未取得对象数据库信息。");
        }
        protected void ValidateId()
        {
            RuleFor(c => c.ID)
                .NotEqual(0).WithErrorCode(ErrorCode.Info);
        }
    }
}
