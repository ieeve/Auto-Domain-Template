using MapsterMapper;
using Modules.Core.AppServices.Authentication;
using Modules.CodeGenerator.Template.Domain.CodeTemplate;
using Modules.CodeGenerator.Template.Domain.DbEntity;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;

namespace Modules.CodeGenerator.Template.AppServices.CodeTemplate
{
    public class CodeTemplateService : ICodeTemplateService
    {
        private readonly ICodeTemplateRepository _repository;
        private ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        public CodeTemplateService(ICodeTemplateRepository Repository, ICurrentUserService CurrentUser, IMapper mapper)
        {
            this._repository = Repository;
            this._currentUserService = CurrentUser;
            this._mapper = mapper;
        }
        #region 查询
        public async Task<CodeTemplateVM> QueryVmByIdAsync(string Id)
        {
            var info = await _repository.QueryByIdAsync(Id);
            return _mapper.Map<CodeTemplateVM>(info);
        }
        #endregion
    }
}
