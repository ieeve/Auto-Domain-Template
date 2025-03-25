using Modules.Core.Domain.TableDataModel;
using Modules.Core.Shared;
using Modules.Template.Shared.CodeTemplate;

namespace Modules.Template.AppServices.CodeTemplate
{
    public interface ICodeTemplatePageService
    {
        /*代码生成器会覆盖标记之间的内容。*/
        //CodeGenerator start
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
        void SetTableModelDataSource(List<CodeTemplateVM> list);
        Task<List<Dictionary<string, object>>> QueryTopPageDataAsync();
        Task<List<Dictionary<string, object>>> QueryCurrentPageDataAsync();
        //CodeGenerator end
    }
}
