using MapsterMapper;
using Modules.My.Domain.CodeTemplate;
using Modules.My.Domain.DbEntity;

namespace Modules.My.Repository.CodeTemplate
{
    public class CodeTemplateTreeRepository : BaseRepository<CodeTemplateTreeEntity>, ICodeTemplateTreeRepository
    {
        private readonly IMapper _mapper;
        public CodeTemplateTreeRepository(SqlSugar.ISqlSugarClient _DbClient, IMapper mapper) : base(_DbClient)
        {
            _mapper = mapper;
        }
    }
}
