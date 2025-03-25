using MapsterMapper;
using Modules.Core.AppServices.Authentication;
using Modules.Core.Domain.TableModels;
using Modules.Template.Domain.CodeTemplate;
using Modules.Template.Domain.DbEntity;
using Modules.Template.Shared.CodeTemplate;

namespace Modules.Template.AppServices.CodeTemplate
{
    public class CodeTemplateSimplePageService : ICodeTemplateSimplePageService
    {
        private readonly ICodeTemplateRepository _repository;
        private ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CodeTemplateSimplePageService(ICodeTemplateRepository Repository, ICurrentUserService CurrentUser, IMapper mapper)
        {
            this._currentUserService = CurrentUser;
            this._repository = Repository;
            this._mapper = mapper;
        }
        #region 查询
        public async Task<CodeTemplateVM> QueryVmByIdAsync(string Id)
        {
            var info = await _repository.QueryByIdAsync(Id);
            return _mapper.Map<CodeTemplateVM>(info);
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="TModel"></param>
        /// <returns></returns>
        public async Task<(List<CodeTemplateVM>, int TotalCount)> GetSimplePageData(TableModel<CodeTemplateVM> TModel)
        {
            var data = await _repository.QueryPageAsync(TModel.PageIndex, TModel.PageSize, TModel.ConditionalModels, TModel.OrderByModel);
            var list = _mapper.Map<List<CodeTemplateVM>>(data.Data);
            return (list, data.TotalCount);
        }
        #endregion

        #region 添加，删除，修改
        public async Task<bool> RemoveRowDataAsync(CodeTemplateVM row)
        {
            return await _repository.DeleteByIdAsync(row.Id);
        }
        public async Task<bool> RemoveRowDataAsync(List<CodeTemplateVM> rows)
        {
            var IDs = rows.Select(s => s.Id).ToArray();
            return await _repository.DeleteByIdsAsync(IDs);
        }
        public async Task<CodeTemplateVM?> AddRowDataAsync(CodeTemplateVM row)
        {
            row.Id = YitIdHelper.NextId().ToString();
            row.Createtime = DateTime.Now;
            row.Createuid = _currentUserService.CurrentUser.Authname;
            var ret = await _repository.InsertAsync(_mapper.Map<CodeTemplateEntity>(row));
            return ret > 0 ? row : null;
        }
        public async Task<List<CodeTemplateVM>?> AddRowDataAsync(List<CodeTemplateVM> rows)
        {
            foreach (var row in rows)
            {
                row.Id = YitIdHelper.NextId().ToString();
                row.Createtime = DateTime.Now;
                row.Createuid = _currentUserService.CurrentUser.Authname;
            }

            var ret = await _repository.InsertAsync(_mapper.Map<List<CodeTemplateEntity>>(rows));
            return ret > 0 ? rows : null;
        }
        public async Task<bool> UpdateRowDataAsync(CodeTemplateVM row)
        {
            row.Updatetime = DateTime.Now;
            row.Updateuid = _currentUserService.CurrentUser.Authname;
            return await _repository.UpdateAsync(_mapper.Map<CodeTemplateEntity>(row));
        }
        public async Task<bool> UpdateRowDataAsync(List<CodeTemplateVM> rows)
        {
            foreach (var row in rows)
            {
                row.Updatetime = DateTime.Now;
                row.Updateuid = _currentUserService.CurrentUser.Authname;
            }
            return await _repository.UpdateAsync(_mapper.Map<List<CodeTemplateEntity>>(rows));
        }
        #endregion
    }
}
