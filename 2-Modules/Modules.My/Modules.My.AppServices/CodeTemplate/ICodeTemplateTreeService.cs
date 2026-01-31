using Modules.Core.Domain.TableModels;
using Modules.My.Shared.CodeTemplate;

namespace Modules.My.AppServices.CodeTemplate
{
    public interface ICodeTemplateTreeService
    {
        Task<CodeTemplateTreeVM?> AddRowDataAsync(CodeTemplateTreeVM row);
        Task<List<CodeTemplateTreeVM>?> AddRowDataAsync(List<CodeTemplateTreeVM> rows);
        Task<List<CodeTemplateTreeVM>> QueryAllAsync();
        Task<(List<CodeTemplateTreeVM>, int TotalCount)> QueryTreeDataAsync(TableModel<CodeTemplateTreeVM> TModel);
        Task<CodeTemplateTreeVM> QueryVmByIdAsync(string Id);
        Task<bool> RemoveRowDataAsync(CodeTemplateTreeVM row);
        Task<bool> RemoveRowDataAsync(List<CodeTemplateTreeVM> rows);
        Task<bool> UpdateRowDataAsync(CodeTemplateTreeVM row);
        Task<bool> UpdateRowDataAsync(List<CodeTemplateTreeVM> rows);
    }
}
