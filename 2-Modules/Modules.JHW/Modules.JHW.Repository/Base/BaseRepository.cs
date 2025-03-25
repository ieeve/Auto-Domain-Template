using Modules.Core.Repository.Base;
using Modules.JHW.Domain.Base;
using SqlSugar;

namespace Modules.JHW.Repository.Base
{
    public class BaseRepository<TEntity> : CoreRepository<TEntity>, IBaseRepository<TEntity> where TEntity : class, new()
    {
        public BaseRepository(ISqlSugarClient _DbClient) : base(_DbClient)
        {
            //指定模块使用的数据库
            //base.SetDbClient(id);//CoreDbConfig.json 里的数据库id
            base.SetDbClient(); //使用默认库
        }
    }
}
