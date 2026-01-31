using Modules.Core.Domain.TableModels;
using Modules.My.Shared.CodeTemplate;

namespace Modules.My.AppServices.CodeTemplate
{
    public interface ICodeTemplateSimplePageService
    {
        Task<CodeTemplateVM?> AddRowDataAsync(CodeTemplateVM row);
        Task<List<CodeTemplateVM>?> AddRowDataAsync(List<CodeTemplateVM> rows);
        Task<(List<CodeTemplateVM>, int TotalCount)> GetSimplePageData(TableModel<CodeTemplateVM> TModel);
        Task<CodeTemplateVM> QueryVmByIdAsync(string Id);
        Task<bool> RemoveRowDataAsync(CodeTemplateVM row);
        Task<bool> RemoveRowDataAsync(List<CodeTemplateVM> rows);
        Task<bool> UpdateRowDataAsync(CodeTemplateVM row);
        Task<bool> UpdateRowDataAsync(IEnumerable<CodeTemplateVM> rows);
    }
}
