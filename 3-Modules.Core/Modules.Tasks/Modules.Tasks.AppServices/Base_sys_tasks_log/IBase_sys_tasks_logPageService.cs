using Modules.Core.Domain.TableDataModel;
using Modules.Core.Shared;
using Modules.Tasks.Shared.Base_sys_tasks_log;

namespace Modules.Tasks.AppServices.Base_sys_tasks_log
{
    public interface IBase_sys_tasks_logPageService
    {
        /*代码生成器会覆盖标记之间的内容。*/
        //CodeGenerator start
        TableDataModel<Base_sys_tasks_logVM> TableDataModel { get; }
        //UserTableColumnModel UserColumnModel { get; }
        Task QueryPageDataAsync();
        Task<bool> RemoveRowDataAsync(Base_sys_tasks_logVM row);
        Task<bool> RemoveRowDataAsync(List<Base_sys_tasks_logVM> row);
        Task<bool> AddRowDataAsync(Base_sys_tasks_logVM row);
        Task<bool> AddRowDataAsync(List<Base_sys_tasks_logVM> rows);
        Task<bool> UpdateRowDataAsync(Base_sys_tasks_logVM row);
        Task<bool> UpdateRowDataAsync(List<Base_sys_tasks_logVM> rows);
        Task InitAsync();
        Task QueryPageTotalCountAsync();
        void SetTableModelDataSource(List<Base_sys_tasks_logVM> list);
        Task<List<Dictionary<string, object>>> QueryTopPageDataAsync();
        Task<List<Dictionary<string, object>>> QueryCurrentPageDataAsync();
        //CodeGenerator end
    }
}