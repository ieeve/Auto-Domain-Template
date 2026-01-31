using MapsterMapper;
using Modules.My.Domain.CodeTemplate;
using Modules.My.Domain.DbEntity;
using Modules.My.Shared.CodeTemplate;
using Modules.Core.AppServices.AppConfig;
using Modules.Core.AppServices.Authentication;
using Modules.Core.AppServices.Base_enum_data;
using Modules.Core.AppServices.Base_log;
using Modules.Core.AppServices.Base_object;
using Modules.Core.AppServices.Base_object_event;
using Modules.Core.AppServices.Base_user_columns;
using Modules.Core.Domain.Converter;
using Modules.Core.Domain.DomainServices.ColumnHeader;
using Modules.Core.Domain.ObjectDomain;
using Modules.Core.Domain.TableDataModel;
using Modules.Core.Shared.Base_object_event;

namespace Modules.My.AppServices.CodeTemplate
{
    public class CodeTemplatePageService : ICodeTemplatePageService
    {
        private readonly ICodeTemplateRepository _repository;
        private ICurrentUserService _currentUserService;
        private IBase_user_columnsService user_ColumnsService;
        private IBase_logService logService;
        private IBase_objectService objectService;
        private readonly IMapper _mapper;
        private IAppConfigServices appConfigService;
        private IBase_enum_dataService enum_DataService;
        private IBase_object_eventService objectEventService;
        public TableDataModel<CodeTemplateVM> TableDataModel { get; private set; }
        private ObjectModel ObjectModel = new ObjectModel();
        public CodeTemplatePageService(IMapper mapper, IBase_object_eventService ObjectEventService, IBase_enum_dataService Enum_DataService, IAppConfigServices AppConfigService, IBase_objectService objectService, ICodeTemplateRepository Repository, IBase_logService logService, IBase_user_columnsService user_ColumnsService, ICurrentUserService CurrentUser)
        {
            this.objectService = objectService;
            this._currentUserService = CurrentUser;
            this.user_ColumnsService = user_ColumnsService;
            this.logService = logService;
            _repository = Repository;
            this._mapper = mapper;
            this.appConfigService = AppConfigService;
            this.enum_DataService = Enum_DataService;
            this.objectEventService = ObjectEventService;

            //在构造函数初始化TableDataModel
            var table_name = "CodeTemplate";
            this.TableDataModel = new TableDataModel<CodeTemplateVM>(table_name, Repository.DbClient.CurrentConnectionConfig);
        }
        public async Task InitAsync()
        {
            this.ObjectModel = await objectService.GetObjectModel(TableDataModel.Db_Id, TableDataModel.TableName);
            if (this.ObjectModel.IsValid())
            {
                //ColumnHeaderModel从对象取得
                this.TableDataModel.SetColumnHeaderModels(ColumnHeaderService.ProcessColumnHeaderModels(ObjectModel.ObjectInfo, ObjectModel.PropertyList, _currentUserService.UserModel.UserRoles));
            }
            else
            {
                //ColumnHeaderModel需要从数据库实体获得，，或者从sql查询结果获得，这样才能与数据表一一对应
                this.TableDataModel.SetColumnHeaderModels(ColumnHeaderService.GetColumnHeaderModelFromVM<CodeTemplateEntity>(TableDataModel.TableName));
            }
            //合并用户自定义列
            this.TableDataModel.SetUserColumnsVMs(await user_ColumnsService.GetUerTableColumn(this.TableDataModel.ColumnHeaderModels));
            //配置枚举列
            this.TableDataModel.EnumDataList = await enum_DataService.GetEnmDataByTableName(TableDataModel.Db_Id, TableDataModel.TableName, TableDataModel.ColumnHeaderModels);
        }

        #region EXCEL导入导出
        public async Task<List<Dictionary<string, object>>> QueryCurrentPageDataAsync()
        {
            var sql = this.TableDataModel.GetPageSql(); //取得当前页
            return await _repository.QueryPageDictDataAsync(sql.sql, sql.SqlParameters);
        }
        public async Task<List<Dictionary<string, object>>> QueryTopPageDataAsync()
        {
            var AppConfig = await appConfigService.GetAppConfig();
            var sql = this.TableDataModel.GetTopPageSql(AppConfig.Max_exportexcel); //取得当前筛选条件下的top数据
            return await _repository.QueryPageDictDataAsync(sql.sql, sql.SqlParameters);
        }
        #endregion

        #region 分页查询(sql方式)
        public async Task QueryPageDataAsync()
        {
            var sql = this.TableDataModel.GetPageSql();
            var data = await _repository.QueryPageDataAsync(sql.sql, sql.SqlParameters);
            this.TableDataModel.TableModel.DataSource = _mapper.Map<List<CodeTemplateVM>>(data);
        }
        public async Task QueryPageTotalCountAsync()
        {
            var sql = this.TableDataModel.GetPageCountSql();
            int total = await _repository.QueryPageTotalCountAsync(sql.sql, sql.SqlParameters);
            this.TableDataModel.TableModel.TotalCount = total;
        }
        #endregion

        #region 添加，删除，修改
        public async Task<bool> RemoveRowDataAsync(CodeTemplateVM row)
        {
            //不验证，牺牲性能保持读取简单
            var beforeData = await _repository.QueryByIdAsync(row.Id);
            var ret = await _repository.DeleteByIdAsync(row.Id);
            if (ret)
            {
                //记录删除日志
                await logService.AddDeleteLog(ObjectModel.ObjectInfo, DictionaryConverter.ModelToDictionary(beforeData));
                //发送通知事件
                await objectEventService.Publish_NotifyEvent(ObjectModel.DbInfo.Id, ObjectModel.ObjectInfo.Name_short, ObjectModel.PrimaryName, CONST_object_event.Trigger_type.删除, DictionaryConverter.ModelToDictionary(beforeData), null);
            }
            return ret;
        }
        public async Task<bool> RemoveRowDataAsync(List<CodeTemplateVM> rows)
        {
            var IDs = rows.Select(s => s.Id).ToArray();
            //不验证，牺牲性能保持读取简单
            var beforeData = await _repository.QueryByIDsAsync(IDs);
            var ret = await _repository.DeleteByIdsAsync(IDs);
            if (ret)
            {
                //记录删除日志
                await logService.AddDeleteLog(ObjectModel.ObjectInfo, DictionaryConverter.ModelToDictionary(beforeData));
                //发送通知事件
                await objectEventService.Publish_NotifyEvent(ObjectModel.DbInfo.Id, ObjectModel.ObjectInfo.Name_short, ObjectModel.PrimaryName, CONST_object_event.Trigger_type.删除, DictionaryConverter.ModelToDictionary(beforeData), null);
            }
            return ret;
        }
        public async Task<CodeTemplateVM?> AddRowDataAsync(CodeTemplateVM row)
        {
            row.Id = IdGenerator.NanoidHelper.NextId();
            row.Createtime = DateTime.Now;
            row.Createuid = _currentUserService.UserModel.Authname;
            var ret = await _repository.InsertAsync(_mapper.Map<CodeTemplateEntity>(row));
            if (ret > 0)
            {
                //发送通知事件
                await objectEventService.Publish_NotifyEvent(ObjectModel.DbInfo.Id, ObjectModel.ObjectInfo.Name_short, ObjectModel.PrimaryName, CONST_object_event.Trigger_type.新增, null, DictionaryConverter.ModelToDictionary(row));
                return row;
            }
            else
            {
                return null;
            }
        }
        public async Task<bool> AddRowDataAsync(List<CodeTemplateVM> rows)
        {
            foreach (var row in rows)
            {
                row.Id = IdGenerator.NanoidHelper.NextId();
                row.Createtime = DateTime.Now;
                row.Createuid = _currentUserService.UserModel.Authname;
            }

            var ret = await _repository.InsertAsync(_mapper.Map<List<CodeTemplateEntity>>(rows));
            if (ret > 0)
            {
                //发送通知事件
                await objectEventService.Publish_NotifyEvent(ObjectModel.DbInfo.Id, ObjectModel.ObjectInfo.Name_short, ObjectModel.PrimaryName, CONST_object_event.Trigger_type.新增, null, DictionaryConverter.ModelToDictionary(rows));
            }
            return ret > 0;
        }
        public async Task<bool> UpdateRowDataAsync(CodeTemplateVM row)
        {
            row.Updatetime = DateTime.Now;
            row.Updateuid = _currentUserService.UserModel.Authname;
            //记录修改日志 不验证，牺牲性能保持读取简单
            var beforeData = await _repository.QueryByIdAsync(row.Id);
            bool ret = await _repository.UpdateAsync(_mapper.Map<CodeTemplateEntity>(row));
            if (ret)
            {
                await logService.AddEditLog(ObjectModel.ObjectInfo, DictionaryConverter.ModelToDictionary(beforeData), DictionaryConverter.ModelToDictionary(row));
                //发送通知事件
                await objectEventService.Publish_NotifyEvent(ObjectModel.DbInfo.Id, ObjectModel.ObjectInfo.Name_short, ObjectModel.PrimaryName, CONST_object_event.Trigger_type.修改, DictionaryConverter.ModelToDictionary(beforeData), DictionaryConverter.ModelToDictionary(row));
            }
            return ret;
        }
        public async Task<bool> UpdateRowDataAsync(List<CodeTemplateVM> rows)
        {
            foreach (var row in rows)
            {
                row.Updatetime = DateTime.Now;
                row.Updateuid = _currentUserService.UserModel.Authname;
            }

            //记录修改日志 不验证，牺牲性能保持读取简单
            var IDs = rows.Select(s => s.Id).ToArray();
            var beforeData = await _repository.QueryByIDsAsync(IDs);

            bool ret = await _repository.UpdateAsync(_mapper.Map<List<CodeTemplateEntity>>(rows));
            if (ret)
            {
                await logService.AddEditLog(ObjectModel.ObjectInfo, DictionaryConverter.ModelToDictionary(beforeData), DictionaryConverter.ModelToDictionary(rows));
                //发送通知事件
                await objectEventService.Publish_NotifyEvent(ObjectModel.DbInfo.Id, ObjectModel.ObjectInfo.Name_short, ObjectModel.PrimaryName, CONST_object_event.Trigger_type.修改, DictionaryConverter.ModelToDictionary(beforeData), DictionaryConverter.ModelToDictionary(rows));
            }
            return ret;
        }
        #endregion

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
