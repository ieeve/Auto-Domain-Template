using MapsterMapper;
using Modules.Template.Domain.CodeTemplate;
using Modules.Template.Domain.DbEntity;

namespace Modules.Template.Repository.CodeTemplate
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
