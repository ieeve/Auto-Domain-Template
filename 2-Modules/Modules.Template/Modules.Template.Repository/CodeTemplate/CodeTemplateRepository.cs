using MapsterMapper;
using Modules.Template.Domain.CodeTemplate;
using Modules.Template.Domain.DbEntity;

namespace Modules.Template.Repository.CodeTemplate
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
