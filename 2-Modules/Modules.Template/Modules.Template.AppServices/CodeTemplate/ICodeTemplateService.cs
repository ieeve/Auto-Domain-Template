using Modules.Template.Domain.DbEntity;
using Modules.Template.Shared.CodeTemplate;

namespace Modules.Template.AppServices.CodeTemplate
{
    public interface ICodeTemplateService
    {
        Task<CodeTemplateVM> QueryVmByIdAsync(string Id);
    }
}
