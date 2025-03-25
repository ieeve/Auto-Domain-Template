using Modules.Core.AppServices.Authentication;
using Modules.Tasks.Domain.Base_sys_tasks_log;
using Modules.Tasks.Domain.DbEntity;
using Modules.Tasks.Shared.Base_sys_tasks_log;

namespace Modules.Tasks.AppServices.Base_sys_tasks_log
{
    public class Base_sys_tasks_logService : BaseServices<Base_sys_tasks_logEntity>, IBase_sys_tasks_logService
    {
        private readonly IBase_sys_tasks_logRepository _repository;
        private ICurrentUserService _currentUserService;

        public Base_sys_tasks_logService(IBase_sys_tasks_logRepository Repository, ICurrentUserService CurrentUser) : base(Repository)
        {
            this._currentUserService = CurrentUser;
            _repository = Repository;
        }

        public async Task<bool> InsertAsync(Base_sys_tasks_logVM vm)
        {
            vm.Id = YitIdHelper.NextId().ToString();
            vm.Createtime = DateTime.Now;
            var ret = await _repository.InsertAsync(_mapper.Map<Base_sys_tasks_logEntity>(vm));
            return ret > 1;
        }
    }
}