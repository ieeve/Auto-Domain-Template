using Modules.CodeGenerator.Template.Domain.DbEntity;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;

namespace Modules.CodeGenerator.Template.AppServices.CodeTemplate
{
    public interface ICodeTemplateService
    {
        Task<CodeTemplateVM> QueryVmByIdAsync(string Id);
    }
}
