using MapsterMapper;
using Modules.Core.AppServices.Authentication;
using Modules.Core.Domain.TableModels;
using Modules.Core.Domain.TreeDataModel;
using Modules.My.Domain.CodeTemplate;
using Modules.My.Domain.DbEntity;
using Modules.My.Shared.CodeTemplate;

namespace Modules.My.AppServices.CodeTemplate
{
    public class CodeTemplateTreeService : ICodeTemplateTreeService
    {
        private readonly ICodeTemplateTreeRepository _repository;
        private ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CodeTemplateTreeService(ICodeTemplateTreeRepository Repository, ICurrentUserService CurrentUser, IMapper mapper)
        {
            this._currentUserService = CurrentUser;
            this._repository = Repository;
            this._mapper = mapper;
        }
        #region 查询
        public async Task<CodeTemplateTreeVM> QueryVmByIdAsync(string Id)
        {
            var info = await _repository.QueryByIdAsync(Id);
            return _mapper.Map<CodeTemplateTreeVM>(info);
        }
        public async Task<List<CodeTemplateTreeVM>> QueryAllAsync()
        {
            var list = await _repository.QueryableAsync();
            return _mapper.Map<List<CodeTemplateTreeVM>>(list);
        }
        /// <summary>
        /// 取得树状结构数据
        /// </summary>
        /// <returns></returns>
        public async Task<(List<CodeTemplateTreeVM>, int TotalCount)> QueryTreeDataAsync(TableModel<CodeTemplateTreeVM> TModel)
        {
            //影响构造树的条件过滤
            object[] inIds = _repository.DbClient.Queryable<CodeTemplateTreeEntity>()
            .Where(TModel.ConditionalModels)
            .Select(it => it.id).ToList().Cast<object>().ToArray();

            var data = await _repository.DbClient.Queryable<CodeTemplateTreeEntity>().OrderBy(TModel.OrderByModel).ToTreeAsync(it => it.SqlsugarTreeChild, it => it.parentid, 0, inIds);

            var TreeData = _mapper.Map<List<CodeTemplateTreeVM>>(data);
            //取得数量
            var list = TreeToListService<CodeTemplateTreeVM>.TreeToList(TreeData);
            return (TreeData, list.Count);
        }

        #endregion

        #region 添加，删除，修改
        public async Task<bool> RemoveRowDataAsync(CodeTemplateTreeVM row)
        {
            return await _repository.DeleteByIdAsync(row.Id);
        }
        public async Task<bool> RemoveRowDataAsync(List<CodeTemplateTreeVM> rows)
        {
            var IDs = rows.Select(s => s.Id).ToArray();
            return await _repository.DeleteByIdsAsync(IDs);
        }
        public async Task<CodeTemplateTreeVM?> AddRowDataAsync(CodeTemplateTreeVM row)
        {
            row.Id = IdGenerator.NanoidHelper.NextId();
            if (string.IsNullOrWhiteSpace(row.Parentid)) row.Parentid = "0";
            row.Createtime = DateTime.Now;
            row.Createuid = _currentUserService.UserModel.Authname;
            var ret = await _repository.InsertAsync(_mapper.Map<CodeTemplateTreeEntity>(row));
            return ret > 0 ? row : null;
        }
        public async Task<List<CodeTemplateTreeVM>?> AddRowDataAsync(List<CodeTemplateTreeVM> rows)
        {
            foreach (var row in rows)
            {
                row.Id = IdGenerator.NanoidHelper.NextId();
                if (string.IsNullOrWhiteSpace(row.Parentid)) row.Parentid = "0";
                row.Createtime = DateTime.Now;
                row.Createuid = _currentUserService.UserModel.Authname;
            }

            var ret = await _repository.InsertAsync(_mapper.Map<List<CodeTemplateTreeEntity>>(rows));
            return ret > 0 ? rows : null;
        }
        public async Task<bool> UpdateRowDataAsync(CodeTemplateTreeVM row)
        {
            if (string.IsNullOrWhiteSpace(row.Parentid)) row.Parentid = "0";
            row.Updatetime = DateTime.Now;
            row.Updateuid = _currentUserService.UserModel.Authname;
            return await _repository.UpdateAsync(_mapper.Map<CodeTemplateTreeEntity>(row));
        }
        public async Task<bool> UpdateRowDataAsync(List<CodeTemplateTreeVM> rows)
        {
            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.Parentid)) row.Parentid = "0";
                row.Updatetime = DateTime.Now;
                row.Updateuid = _currentUserService.UserModel.Authname;
            }
            return await _repository.UpdateAsync(_mapper.Map<List<CodeTemplateTreeEntity>>(rows));
        }
        #endregion
    }
}
