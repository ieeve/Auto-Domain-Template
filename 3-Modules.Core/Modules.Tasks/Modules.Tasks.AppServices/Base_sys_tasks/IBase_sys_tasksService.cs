using Modules.Tasks.Domain.DbEntity;
using Modules.Tasks.Shared.Base_sys_tasks;

namespace Modules.Tasks.AppServices.Base_sys_tasks
{
    public interface IBase_sys_tasksService : IBaseServices<Base_sys_tasksEntity>
    {
        Task<bool> AddRun_timeAsync(string taskId);
        Task<List<Base_sys_tasksVM>> GetAllAsync();
        Task<List<Base_sys_tasksVM>> GetAllStartAsync();
        Task<bool> InsertAsync(Base_sys_tasksVM vm);
        Task<Base_sys_tasksVM> QueryByIdAsync(string id);
        Task<bool> UpdateAsync(Base_sys_tasksVM vm);
    }
}