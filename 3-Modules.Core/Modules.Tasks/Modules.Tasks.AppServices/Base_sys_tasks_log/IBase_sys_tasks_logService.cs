using Modules.Tasks.Domain.DbEntity;
using Modules.Tasks.Shared.Base_sys_tasks_log;

namespace Modules.Tasks.AppServices.Base_sys_tasks_log
{
    public interface IBase_sys_tasks_logService : IBaseServices<Base_sys_tasks_logEntity>
    {
        Task<bool> InsertAsync(Base_sys_tasks_logVM vm);
    }
}