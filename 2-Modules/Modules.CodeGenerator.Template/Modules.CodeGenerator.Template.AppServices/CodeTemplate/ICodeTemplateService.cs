using Modules.CodeGenerator.Template.Domain.DbEntity;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;

namespace Modules.CodeGenerator.Template.AppServices.CodeTemplate
{
    public interface ICodeTemplateService
    {
        Task<CodeTemplateVM> QueryByIdAsync(string Id);
        Task<CodeTemplateVM?> AddRowAsync(CodeTemplateVM row);
        Task<bool> AddRowAsync(List<CodeTemplateVM> rows);
        Task<bool> RemoveRowAsync(CodeTemplateVM row);
        Task<bool> RemoveRowAsync(List<CodeTemplateVM> rows);
        Task<bool> UpdateRowAsync(CodeTemplateVM row);
        Task<bool> UpdateRowAsync(List<CodeTemplateVM> rows);
    }
}
