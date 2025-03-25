using MapsterMapper;
using Modules.Core.AppServices.AppConfig;
using Modules.Core.AppServices.Authentication;
using Modules.Core.AppServices.Base_IdServer;
using Modules.Core.AppServices.Base_log;
using Modules.Core.AppServices.Base_user_columns;
using Modules.Core.AppServices.ObjectServices;
using Modules.Core.Domain.DomainServices.ColumnHeader;
using Modules.Core.Domain.ObjectDomain;
using Modules.Core.Domain.TableDataModel;
using Modules.Core.Shared;
using Modules.Tasks.Domain.Base_sys_tasks_log;
using Modules.Tasks.Domain.DbEntity;
using Modules.Tasks.Shared.Base_sys_tasks_log;
using System.Text.Json;

namespace Modules.Tasks.AppServices.Base_sys_tasks_log
{
    public class Base_sys_tasks_logPageService : IBase_sys_tasks_logPageService
    {
        private readonly IBase_sys_tasks_logRepository _repository;
        private ICurrentUserService _currentUserService;
        private IBase_user_columnsService user_ColumnsService;
        private IBase_logService logService;
        private IObjectService objectService;
        private IMapper _mapper;
        private IAppConfigServices appConfigService;
        public TableDataModel<Base_sys_tasks_logVM> TableDataModel { get; private set; }
        public Core.Domain.ObjectDomain.ObjectModel ObjectModel { get; private set; }
        public Base_sys_tasks_logPageService(IMapper _mapper, IAppConfigServices AppConfigService, IObjectService objectService, IBase_sys_tasks_logRepository Repository, IBase_logService logService, IBase_user_columnsService user_ColumnsService, ICurrentUserService CurrentUser)
        {
            this._currentUserService = CurrentUser;
            this.user_ColumnsService = user_ColumnsService;
            this.logService = logService;
            _repository = Repository;
            this.objectService = objectService;
            this._mapper = _mapper;
            this.appConfigService = AppConfigService;
        }
        public async Task InitAsync()
        {
            var table_name = "Base_sys_tasks_log";
            this.ObjectModel = await objectService.GetObjectModel(_repository.DbClient.CurrentConnectionConfig.ConfigId.ToString(), table_name);
            this.TableDataModel = new TableDataModel<Base_sys_tasks_logVM>(table_name, _repository.DbClient.CurrentConnectionConfig.DbType);

            if (this.ObjectModel.IsValid())
            {
                //����Ƕ���ID���ϲ����Զ���Ľ�ɫȨ��
                this.TableDataModel.SetColumnHeaderModels(ColumnHeaderService.ProcessColumnHeaderModels(ObjectModel.ObjectInfo, ObjectModel.PropertyList, _currentUserService.CurrentUser.UserRoles));

                this.TableDataModel.TableOptions.SetRoleByObject(ObjectModel.ObjectInfo, this.TableDataModel.ColumnHeaderModels, _currentUserService.CurrentUser);
            }
            else
            {
                //�����⻧����Ա�ɱ༭
                this.TableDataModel.TableOptions.SetRoleByTenantAdmin(_currentUserService.CurrentUser);
                //ColumnHeaderModel��Ҫ�����ݿ�ʵ���ã������ߴ�sql��ѯ�����ã��������������ݱ�һһ��Ӧ
                this.TableDataModel.SetColumnHeaderModels(ColumnHeaderService.GetColumnHeaderModelFromVM<Base_sys_tasks_logEntity>(table_name));
            }

            //�ϲ��û��Զ�����
            var user_ColumnsVMs = await user_ColumnsService.GetUerTableColumn(this.TableDataModel.ColumnHeaderModels);
            this.TableDataModel.MergeUserColumnHeaderModel(user_ColumnsVMs);
        }

        /// <summary>
        /// ���÷��������Դ������ҳ����������ʹ��ͬһ������Դ
        /// </summary>
        /// <param name="list"></param>
        public void SetTableModelDataSource(List<Base_sys_tasks_logVM> list)
        {
            if (this.TableDataModel == null) this.TableDataModel = new TableDataModel<Base_sys_tasks_logVM>();
            this.TableDataModel.TableModel.DataSource = list;
        }
        /*�����������Ḳ�Ǳ��֮������ݡ�*/
        //CodeGenerator start

        #region EXCEL���뵼��
        public async Task<List<Dictionary<string, object>>> QueryCurrentPageDataAsync()
        {
            var sql = this.TableDataModel.GetPageSql(); //ȡ�õ�ǰҳ
            return await _repository.QueryPageDictDataAsync(sql.sql, sql.SqlParameters);
        }
        public async Task<List<Dictionary<string, object>>> QueryTopPageDataAsync()
        {
            var AppConfig = await appConfigService.GetAppConfig();
            var sql = this.TableDataModel.GetTopPageSql(AppConfig.Max_exportexcel); //ȡ�õ�ǰɸѡ�����µ�top����
            return await _repository.QueryPageDictDataAsync(sql.sql, sql.SqlParameters);
        }
        #endregion

        #region ��ѯ
        public async Task QueryPageDataAsync()
        {
            var sql = this.TableDataModel.GetPageSql();
            var data = await _repository.QueryPageDataAsync(sql.sql, sql.SqlParameters);
            this.TableDataModel.TableModel.DataSource = _mapper.Map<List<Base_sys_tasks_logVM>>(data);
        }
        public async Task QueryPageTotalCountAsync()
        {
            var sql = this.TableDataModel.GetPageCountSql();
            int total = await _repository.QueryPageTotalCountAsync(sql.sql, sql.SqlParameters);
            this.TableDataModel.TableModel.TotalCount = total;
        }
        #endregion

        #region ��ӣ�ɾ�����޸�
        public async Task<bool> RemoveRowDataAsync(Base_sys_tasks_logVM row)
        {
            //��¼ɾ����־
            Base_sys_tasks_logEntity beforeData = null;
            if (ObjectModel.ObjectInfo != null && ObjectModel.ObjectInfo?.Is_delete_log > 0) beforeData = await _repository.QueryByIdAsync(row.Id);

            var ret = await _repository.DeleteByIdAsync(row.Id);
            if (ret && this.TableDataModel != null)
            {
                this.TableDataModel.TableModel.RemoveRow(row);

                if (ObjectModel.ObjectInfo != null && ObjectModel.ObjectInfo?.Is_delete_log > 0)
                {
                    logService.AddDeleteLog(JsonSerializer.Serialize(beforeData), ObjectModel.ObjectInfo.Solution_id, ObjectModel.ObjectInfo.Id, ObjectModel.ObjectInfo.Name_short);
                }
            }
            return ret;
        }
        public async Task<bool> RemoveRowDataAsync(List<Base_sys_tasks_logVM> rows)
        {
            var IDs = rows.Select(s => s.Id).ToArray();
            //��¼ɾ����־
            List<Base_sys_tasks_logEntity> beforeData = null;
            if (ObjectModel.ObjectInfo != null && ObjectModel.ObjectInfo?.Is_delete_log > 0) beforeData = await _repository.QueryByIDsAsync(IDs);

            var ret = await _repository.DeleteByIdsAsync(IDs);
            if (ret && this.TableDataModel != null)
            {
                this.TableDataModel.TableModel.RemoveRow(rows);
                if (ObjectModel.ObjectInfo != null && ObjectModel.ObjectInfo?.Is_delete_log > 0)
                {
                    //��¼ɾ����־
                    logService.AddDeleteLog(JsonSerializer.Serialize(beforeData), ObjectModel.ObjectInfo.Solution_id, ObjectModel.ObjectInfo.Id, ObjectModel.ObjectInfo.Name_short);
                }
            }
            return ret;
        }
        public async Task<bool> AddRowDataAsync(Base_sys_tasks_logVM row)
        {
            row.Id = YitIdHelper.NextId().ToString();
            row.Createtime = DateTime.Now;
            row.Createuid = _currentUserService.CurrentUser.Authname;
            var ret = await _repository.InsertAsync(_mapper.Map<Base_sys_tasks_logEntity>(row));
            if (ret > 0 && this.TableDataModel != null)
            {
                this.TableDataModel.TableModel.AddRow(row);
            }
            return ret > 0;
        }
        public async Task<bool> AddRowDataAsync(List<Base_sys_tasks_logVM> rows)
        {
            //���û���������������ֵ
            NullIdServer<Base_sys_tasks_logVM>.ProcessNullId(rows);

            foreach (var row in rows)
            {
                row.Id = YitIdHelper.NextId().ToString();
                row.Createtime = DateTime.Now;
                row.Createuid = _currentUserService.CurrentUser.Authname;
            }

            var ret = await _repository.InsertAsync(_mapper.Map<List<Base_sys_tasks_logEntity>>(rows));
            if (ret > 0 && this.TableDataModel != null)
            {
                this.TableDataModel.TableModel.AddRow(rows);
            }
            return ret > 0;
        }
        public async Task<bool> UpdateRowDataAsync(Base_sys_tasks_logVM row)
        {
            row.Updatetime = DateTime.Now;
            row.Updateuid = _currentUserService.CurrentUser.Authname;
            //��¼�޸���־
            Base_sys_tasks_logEntity beforeData = null;
            if (ObjectModel.ObjectInfo != null && ObjectModel.ObjectInfo?.Is_edit_log > 0) beforeData = await _repository.QueryByIdAsync(row.Id);

            bool ret = await _repository.UpdateAsync(_mapper.Map<Base_sys_tasks_logEntity>(row));
            if (ret && this.TableDataModel != null)
            {
                this.TableDataModel.TableModel.UpdateRow(row);

                if (ObjectModel.ObjectInfo != null && ObjectModel.ObjectInfo?.Is_edit_log > 0) logService.AddEditLog(JsonSerializer.Serialize(beforeData), JsonSerializer.Serialize(row), ObjectModel.ObjectInfo.Solution_id, ObjectModel.ObjectInfo.Id, ObjectModel.ObjectInfo.Name_short);
            }
            return ret;
        }
        public async Task<bool> UpdateRowDataAsync(List<Base_sys_tasks_logVM> rows)
        {
            foreach (var row in rows)
            {
                row.Updatetime = DateTime.Now;
                row.Updateuid = _currentUserService.CurrentUser.Authname;
            }

            //��¼�޸���־
            var IDs = rows.Select(s => s.Id).ToArray();
            List<Base_sys_tasks_logEntity> beforeData = null;
            if (ObjectModel.ObjectInfo != null && ObjectModel.ObjectInfo?.Is_edit_log > 0) beforeData = await _repository.QueryByIDsAsync(IDs);

            bool ret = await _repository.UpdateAsync(_mapper.Map<List<Base_sys_tasks_logEntity>>(rows));
            if (ret && this.TableDataModel != null)
            {
                this.TableDataModel.TableModel.RemoveRow(rows);

                if (ObjectModel.ObjectInfo != null && ObjectModel.ObjectInfo?.Is_edit_log > 0) logService.AddEditLog(JsonSerializer.Serialize(beforeData), JsonSerializer.Serialize(rows), ObjectModel.ObjectInfo.Solution_id, ObjectModel.ObjectInfo.Id, ObjectModel.ObjectInfo.Name_short);
            }
            return ret;
        }
        #endregion
        //CodeGenerator end
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}