using MapsterMapper;
using Modules.My.Domain.CodeTemplate;
using Modules.My.Domain.DbEntity;

namespace Modules.My.Repository.CodeTemplate
{
    public class CodeTemplateRepository : BaseRepository<CodeTemplateEntity>, ICodeTemplateRepository
    {
        private readonly IMapper _mapper;
        public CodeTemplateRepository(SqlSugar.ISqlSugarClient _DbClient, IMapper mapper) : base(_DbClient)
        {
            _mapper = mapper;
        }
    }
}
