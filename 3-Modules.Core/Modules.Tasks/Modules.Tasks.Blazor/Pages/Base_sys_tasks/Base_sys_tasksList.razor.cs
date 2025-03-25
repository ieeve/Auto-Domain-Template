using AntDesign;
using AntDesign.TableModels;
using Force.DeepCloner;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Modules.Core.AppServices.ExcelServer;
using Modules.Core.Blazor.PageDto;
using Modules.Core.Domain.TableDataModel;
using Modules.Tasks.AppServices.Base_sys_tasks;
using Modules.Tasks.AppServices.TaskScheduler;
using Modules.Tasks.Shared.Base_sys_tasks;

namespace Modules.Tasks.Blazor.Pages.Base_sys_tasks
{
    partial class Base_sys_tasksList
    {
        //ע�����
        [Inject] private IBase_sys_tasksPageService _Service { get; set; }
        [Inject] private ISchedulerCenter _schedulerCenter { get; set; }
        private DialogVM Dialog = new DialogVM();
        private TableDataModel<Base_sys_tasksVM> TableDataModel { get; set; } = new TableDataModel<Base_sys_tasksVM>(); //����ʹ���޲������캯����Ϊ��AntTable��ʼ��

        bool IsAdd = false;
        private Base_sys_tasksVM EditRow = new Base_sys_tasksVM();

        private void OnValueCallback(bool isSuccess)
        {
            Dialog.Visible = false;
        }
        //��������ɸѡ
        int? FilterCount { get => TableDataModel.ColumnHeaderModels.Count(s => !string.IsNullOrWhiteSpace(s.search)); }
        public async Task ResetTable()
        {
            TableDataModel.ColumnHeaderModels.ForEach(s => s.search = string.Empty);
            await this.FetchPageData();
        }

        protected override async Task OnInitializedAsync()
        {
            await _Service.InitAsync();

            this.TableDataModel = _Service.TableDataModel;

            await base.OnInitializedAsync();
        }
        //AntDesign Table�����¼�
        async Task HandleTableChange(QueryModel<Base_sys_tasksVM> queryModel)
        {
            TableDataModel.FilterModel = queryModel.FilterModel;
            TableDataModel.SortModel = queryModel.SortModel;
            //����Ĭ��ɸѡ����(���ܵ���)
            //Expression<Func<Base_sys_tasksVM, bool>> expression = s => s.Createuid == "test";
            //TableDataModel.AttachWhereClause(expression); 
            await this.FetchPageData();
        }

        string SavedQueryModel = string.Empty; //��¼��ǰɸѡ��ֵ�������ж��Ƿ���Ҫ��̨��ѯ����
        private async Task FetchPageData()
        {
            TableDataModel.TableModel.Loading = true;
            try
            {
                await _Service.QueryPageDataAsync(); //ȡ�÷�ҳ����
            }
            catch (Exception ex)
            {
                var confirmResult = await _confirmService.Show(ex.StackTrace, ex.Message, ConfirmButtons.OK, ConfirmIcon.Error);
            }
            //��ɸѡ�����ı�ʱ�򣬲���Ҫ����ͳ������
            var searchArr = TableDataModel.ColumnHeaderModels.Select(s => s.search).ToArray();
            var ThisQueryModel = string.Join(",", searchArr);

            if (SavedQueryModel == string.Empty || SavedQueryModel != ThisQueryModel)
            {
                await _Service.QueryPageTotalCountAsync();
                //�����ѯ����
                SavedQueryModel = ThisQueryModel;
            }
            TableDataModel.TableModel.Loading = false;


            //���߳��и����˱���״̬�����ֶ�ˢ�¡�
            // await InvokeAsync(StateHasChanged);
            _ = _message.Success($@"��ǰ��{TableDataModel.TableModel.PageIndex}ҳ,�ϼ�{TableDataModel.TableModel.TotalCount}�����ݡ�");

        }
        private async Task OnInputClearChange(string value, string field)
        {
            // ��ɸѡ�����ı�ʱ�򣬲���Ҫ����ͳ������
            var searchArr = TableDataModel.ColumnHeaderModels.Select(s => s.search).ToArray();
            var ThisQueryModel = string.Join(",", searchArr);

            if (SavedQueryModel == string.Empty || SavedQueryModel != ThisQueryModel)
            {
                await this.FetchPageData();
            }
        }

        public async Task OnStopAsync(Base_sys_tasksVM model)
        {
            var confirmResult = await _confirmService.Show("ȷ��ֹͣ��������", "", ConfirmButtons.OKCancel);
            if (confirmResult == ConfirmResult.OK)
            {
                var ResuleModel = await _schedulerCenter.StopScheduleJobAsync(model);
                if (ResuleModel.Code == Core.Shared.ResultType.Success)
                {
                    model.Is_start = false;
                    var ret = await _Service.UpdateRowDataAsync(model);
                    if (ret)
                    {
                        _ = _message.Success("����ֹͣ�ɹ� ��");
                    }
                    else
                    {
                        _ = _message.Error("����ʧ��,");
                    }
                }
                else
                {
                    _ = _message.Error("����ֹͣʧ�ܣ�" + ResuleModel.Msg);
                }
            }
        }
        public async Task OnStart(Base_sys_tasksVM model)
        {
            if (model.Is_start)
            {
                _ = _message.Info("�������Ѿ��������޷��ٴ����� ��");
                return;
            }

            var ResuleModel = await _schedulerCenter.AddScheduleJobAsync(model);
            if (ResuleModel.Code == Core.Shared.ResultType.Success)
            {
                model.Is_start = true;
                model.Updatetime = DateTime.Now;
                var ret = await _Service.UpdateRowDataAsync(model);
                if (ret)
                {
                    _ = _message.Success("���������ɹ� ��");
                }
                else
                {
                    _ = _message.Error("����ʧ��,");
                }
            }
            else
            {
                _ = _message.Error("��������ʧ�ܣ�" + String.Join(',', ResuleModel.Msg));
            }

        }
        /// <summary>
        /// ��ҳ�ص�
        /// </summary>
        /// <param name="args"></param>
        protected virtual async Task PageIndexChanged(PaginationEventArgs args)
        {
            if (TableDataModel.TableModel.PageIndex != args.Page || TableDataModel.TableModel.PageSize != args.PageSize)
            {
                //�������
                TableDataModel.TableModel.PageIndex = args.Page;
                TableDataModel.TableModel.PageSize = args.PageSize;
                await this.FetchPageData();
            }
        }
        #region ����excel
        [Inject] private ExportExcelObjectData excelService { get; set; }
        async Task ExportExcelClick(bool IsAllData)
        {
            if (IsAllData)
            {
                var data = await _Service.QueryTopPageDataAsync();
                await excelService.ExportExcelData(data, TableDataModel.ColumnHeaderModels);
            }
            else
            {
                var data = await _Service.QueryCurrentPageDataAsync();
                await excelService.ExportExcelData(data, TableDataModel.ColumnHeaderModels);
            }
        }
        #endregion
        #region ���ñ���ȣ�Ĭ����ʾ�е���Ϣ��
        private bool userColumnDrawerVisible = false;
        private void SettingTable()
        {
            this.userColumnDrawerVisible = true;
        }
        //�û��Զ����з����¼�
        private void OnUserColumnDrawerCallback()
        {
            this.userColumnDrawerVisible = false;
        }
        #endregion

        #region ��ɾ��
        private void AddClick()
        {
            this.IsAdd = true;
            EditRow.Tenant_id = _CurrentUserService.CurrentUser.Tenant_id;
            Dialog.Title = "�������";
            Dialog.Visible = true;
        }
        private void EditClick(Base_sys_tasksVM editRow)
        {
            this.IsAdd = false;
            this.EditRow = editRow.DeepClone();
            Dialog.Title = "�༭����:" + this.EditRow.Id;
            Dialog.Visible = true;
        }
        private void DelClick(Base_sys_tasksVM editRow)
        {
            if (editRow.Is_start)
            {
                _ = _message.Success("������Ҫֹͣ�󣬲���ɾ����");
                return;
            }
            _modalService.Confirm(new ConfirmOptions()
            {

                Content = "ȷ��ɾ������������",
                OnOk = async (e) =>
                {
                    var ret = await _Service.RemoveRowDataAsync(editRow);
                    if (ret)
                    {
                        await InvokeAsync(StateHasChanged); //ǿ��ˢ��
                        _ = _message.Success("ɾ���ɹ�");
                    }
                    else
                    {
                        _ = _message.Error("ɾ��ʧ�ܡ�");
                    }
                },
                OnCancel = async (e) =>
                {
                    _ = _message.Info("�û�ȡ��ɾ����");
                }
            });
        }

        private void BatchDelClick()
        {
            if (TableDataModel.TableModel.SelectedRows == null || TableDataModel.TableModel.SelectedRows.Count() == 0)
            {
                _message.Info("��ѡ����Ҫɾ�������ݡ�");
                return;
            }
            if (TableDataModel.TableModel.SelectedRows.FirstOrDefault(s => s.Is_start) != null)
            {
                _ = _message.Success("������Ҫֹͣ�󣬲���ɾ����");
                return;
            }
            var ret = false;
            _modalService.ConfirmAsync(new ConfirmOptions()
            {

                Content = $@"ȷ��ɾ����{TableDataModel.TableModel.SelectedRows.Count()}��������",
                OnOk = async (e) =>
                {
                    ret = await _Service.RemoveRowDataAsync(TableDataModel.TableModel.SelectedRows.ToList());
                    if (ret)
                    {
                        await InvokeAsync(StateHasChanged); //ǿ��ˢ��
                        _ = _message.Success("ɾ���ɹ�");
                    }
                    else
                    {
                        _ = _message.Error("ɾ��ʧ�ܡ�");
                    }
                },
                OnCancel = async (e) =>
                {
                    _ = _message.Info("�û�ȡ��ɾ����");
                }
            });
        }
        #endregion
    }
}