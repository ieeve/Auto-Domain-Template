using Modules.My.Domain.DbEntity;
using Modules.My.Shared.CodeTemplate;

namespace Modules.My.AppServices.CodeTemplate
{
    public interface ICodeTemplateService
    {
        Task<CodeTemplateVM> QueryVmByIdAsync(string Id);
    }
}
