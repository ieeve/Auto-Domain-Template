
using MapsterMapper;
using Modules.Tasks.Domain.Base_sys_tasks;
using Modules.Tasks.Domain.DbEntity;

namespace Modules.Tasks.Repository.Base_sys_tasks
{
    public class Base_sys_tasksRepository : BaseRepository<Base_sys_tasksEntity>, IBase_sys_tasksRepository
    {
        private readonly IMapper _mapper;
        public Base_sys_tasksRepository(SqlSugar.ISqlSugarClient _DbClient, IMapper mapper) : base(_DbClient)
        {
            _mapper = mapper;
        }
    }
}
