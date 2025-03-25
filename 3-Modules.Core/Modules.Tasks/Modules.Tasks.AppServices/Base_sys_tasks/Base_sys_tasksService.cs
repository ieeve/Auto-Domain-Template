using MathNet.Numerics.Distributions;
using Modules.Core.AppServices.Authentication;
using Modules.Tasks.Domain.Base_sys_tasks;
using Modules.Tasks.Domain.DbEntity;
using Modules.Tasks.Shared.Base_sys_tasks;
using SqlSugar;

namespace Modules.Tasks.AppServices.Base_sys_tasks
{
    public class Base_sys_tasksService : BaseServices<Base_sys_tasksEntity>, IBase_sys_tasksService
    {
        private readonly IBase_sys_tasksRepository _repository;
        private ICurrentUserService _currentUserService;

        public Base_sys_tasksService(IBase_sys_tasksRepository Repository, ICurrentUserService CurrentUser) : base(Repository)
        {
            this._currentUserService = CurrentUser;
            _repository = Repository;
        }
        public async Task<List<Base_sys_tasksVM>> GetAllAsync()
        {
            var ret = await _repository.QueryableAsync();
            return _mapper.Map<List<Base_sys_tasksVM>>(ret);
        }
        public async Task<List<Base_sys_tasksVM>> GetAllStartAsync()
        {
            var ret = await _repository.QueryableAsync(s => s.IS_START);
            return _mapper.Map<List<Base_sys_tasksVM>>(ret);
        }
        public async Task<Base_sys_tasksVM> QueryByIdAsync(string id)
        {
            var ret = await _repository.QueryByIdAsync(id);
            return _mapper.Map<Base_sys_tasksVM>(ret);
        }
        public async Task<bool> UpdateAsync(Base_sys_tasksVM vm)
        {
            vm.Updatetime = DateTime.Now;
            vm.Updateuid = _currentUserService.CurrentUser.Authname;
            var ret = await _repository.UpdateAsync(_mapper.Map<Base_sys_tasksEntity>(vm));
            return ret;
        }
        public async Task<bool> InsertAsync(Base_sys_tasksVM vm)
        {
            vm.Id = YitIdHelper.NextId().ToString();
            vm.Createtime = DateTime.Now;
            vm.Createuid = _currentUserService.CurrentUser.Authname;
            var ret = await _repository.InsertAsync(_mapper.Map<Base_sys_tasksEntity>(vm));
            return ret > 0;
        }

        public async Task<bool> AddRun_timeAsync(string taskId)
        {
            /*
            var param = new SugarParameter("@id", taskId);
            string sql = $@"update base_sys_tasks set run_times = run_times+1 where id = @id";
            var ret = await _repository.DbClient.Ado.ExecuteCommandAsync(sql, param);
            */
            var ret = await _repository.DbClient.Updateable<Base_sys_tasksEntity>()
.SetColumns(it => it.RUN_TIMES == it.RUN_TIMES + 1)
.SetColumns(it => it.LAST_SUCCESS_TIME == DateTime.Now)
.Where(it => it.ID == taskId)
.ExecuteCommandAsync();

            return ret > 0;
        }
    }
}