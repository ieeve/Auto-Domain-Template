using MapsterMapper;
using Modules.CodeGenerator.Template.Domain.CodeTemplate;
using Modules.CodeGenerator.Template.Domain.DbEntity;

namespace Modules.CodeGenerator.Template.Repository.CodeTemplate
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
