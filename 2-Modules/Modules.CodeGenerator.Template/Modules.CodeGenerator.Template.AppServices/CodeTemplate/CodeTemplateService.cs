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
        public async Task<CodeTemplateVM> QueryByIdAsync(string Id)
        {
            var info = await _repository.QueryByIdAsync(Id);
            return _mapper.Map<CodeTemplateVM>(info);
        }
        #endregion
        #region 增删改
        public async Task<CodeTemplateVM?> AddRowAsync(CodeTemplateVM row)
        {
            row.Id = IdGenerator.NanoidHelper.NextId();
            row.Createtime = DateTime.Now;
            row.Createuid = _currentUserService.UserModel.Authname;
            var ret = await _repository.InsertAsync(_mapper.Map<CodeTemplateEntity>(row));
            return ret > 0 ? row : null;
        }
        public async Task<bool> AddRowAsync(List<CodeTemplateVM> rows)
        {
            foreach (var row in rows)
            {
                row.Id = IdGenerator.NanoidHelper.NextId();
                row.Createtime = DateTime.Now;
                row.Createuid = _currentUserService.UserModel.Authname;
            }
            var ret = await _repository.InsertAsync(_mapper.Map<List<CodeTemplateEntity>>(rows));
            return ret > 0;
        }
        public async Task<bool> RemoveRowAsync(CodeTemplateVM row)
        {
            var ret = await _repository.DeleteByIdAsync(row.Id);
            return ret;
        }
        public async Task<bool> RemoveRowAsync(List<CodeTemplateVM> rows)
        {
            var IDs = rows.Select(s => s.Id).ToArray();
            var ret = await _repository.DeleteByIdsAsync(IDs);
            return ret;
        }

        public async Task<bool> UpdateRowAsync(CodeTemplateVM row)
        {
            row.Updatetime = DateTime.Now;
            row.Updateuid = _currentUserService.UserModel.Authname;
            bool ret = await _repository.UpdateAsync(_mapper.Map<CodeTemplateEntity>(row));
            return ret;
        }
        public async Task<bool> UpdateRowAsync(List<CodeTemplateVM> rows)
        {
            foreach (var row in rows)
            {
                row.Updatetime = DateTime.Now;
                row.Updateuid = _currentUserService.UserModel.Authname;
            }
            bool ret = await _repository.UpdateAsync(_mapper.Map<List<CodeTemplateEntity>>(rows));
            return ret;
        }
        #endregion
    }
}
