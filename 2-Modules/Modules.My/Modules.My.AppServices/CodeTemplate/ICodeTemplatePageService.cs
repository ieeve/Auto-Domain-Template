using Modules.My.Shared.CodeTemplate;
using Modules.Core.Domain.TableDataModel;

namespace Modules.My.AppServices.CodeTemplate
{
    public interface ICodeTemplatePageService
    {
        TableDataModel<CodeTemplateVM> TableDataModel { get; }
        Task QueryPageDataAsync();
        Task<bool> RemoveRowDataAsync(CodeTemplateVM row);
        Task<bool> RemoveRowDataAsync(List<CodeTemplateVM> row);
        Task<CodeTemplateVM?> AddRowDataAsync(CodeTemplateVM row);
        Task<bool> AddRowDataAsync(List<CodeTemplateVM> rows);
        Task<bool> UpdateRowDataAsync(CodeTemplateVM row);
        Task<bool> UpdateRowDataAsync(List<CodeTemplateVM> rows);
        Task InitAsync();
        Task QueryPageTotalCountAsync();
        Task<List<Dictionary<string, object>>> QueryTopPageDataAsync();
        Task<List<Dictionary<string, object>>> QueryCurrentPageDataAsync();
    }
}
