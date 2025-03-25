
using MapsterMapper;
using Modules.Tasks.Domain.Base_sys_tasks_log;
using Modules.Tasks.Domain.DbEntity;

namespace Modules.Tasks.Repository.Base_sys_tasks_log
{
    public class Base_sys_tasks_logRepository : BaseRepository<Base_sys_tasks_logEntity>, IBase_sys_tasks_logRepository
    {
        private readonly IMapper _mapper;
        public Base_sys_tasks_logRepository(SqlSugar.ISqlSugarClient _DbClient, IMapper mapper) : base(_DbClient)
        {
            _mapper = mapper;
        }
    }
}
