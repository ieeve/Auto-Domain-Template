using MapsterMapper;
using Modules.CodeGenerator.Template.Domain.CodeTemplate;
using Modules.CodeGenerator.Template.Domain.DbEntity;

namespace Modules.CodeGenerator.Template.Repository.CodeTemplate
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
