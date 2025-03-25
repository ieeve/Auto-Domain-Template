using Modules.Core.Domain.TableDataModel;
using Modules.Core.Shared;
using Modules.Tasks.Shared.Base_sys_tasks;

namespace Modules.Tasks.AppServices.Base_sys_tasks
{
    public interface IBase_sys_tasksPageService
    {
        /*代码生成器会覆盖标记之间的内容。*/
        //CodeGenerator start
        TableDataModel<Base_sys_tasksVM> TableDataModel { get; }
        //UserTableColumnModel UserColumnModel { get; }
        Task QueryPageDataAsync();
        Task<bool> RemoveRowDataAsync(Base_sys_tasksVM row);
        Task<bool> RemoveRowDataAsync(List<Base_sys_tasksVM> row);
        Task<bool> AddRowDataAsync(Base_sys_tasksVM row);
        Task<bool> AddRowDataAsync(List<Base_sys_tasksVM> rows);
        Task<bool> UpdateRowDataAsync(Base_sys_tasksVM row);
        Task<bool> UpdateRowDataAsync(List<Base_sys_tasksVM> rows);
        Task InitAsync();
        Task QueryPageTotalCountAsync();
        void SetTableModelDataSource(List<Base_sys_tasksVM> list);
        Task<List<Dictionary<string, object>>> QueryTopPageDataAsync();
        Task<List<Dictionary<string, object>>> QueryCurrentPageDataAsync();
        //CodeGenerator end
    }
}