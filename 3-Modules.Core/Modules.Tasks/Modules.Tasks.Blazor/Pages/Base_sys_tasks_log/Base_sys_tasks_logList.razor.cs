using AntDesign;
using AntDesign.TableModels;
using Force.DeepCloner;
using Microsoft.AspNetCore.Components;
using Modules.Core.AppServices.ExcelServer;
using Modules.Core.Blazor.PageDto;
using Modules.Core.Domain.TableDataModel;
using Modules.Tasks.AppServices.Base_sys_tasks_log;
using Modules.Tasks.Shared.Base_sys_tasks_log;
using NPOI.SS.Formula.Functions;

namespace Modules.Tasks.Blazor.Pages.Base_sys_tasks_log
{
    partial class Base_sys_tasks_logList
    {
        //ע�����
        [Inject] private IBase_sys_tasks_logPageService _Service { get; set; }

        private DialogVM Dialog = new DialogVM();
        private TableDataModel<Base_sys_tasks_logVM> TableDataModel { get; set; } = new TableDataModel<Base_sys_tasks_logVM>(); //����ʹ���޲������캯����Ϊ��AntTable��ʼ��

        bool IsAdd = false;
        private string PageTitleString = "ϵͳ������־";
        private Base_sys_tasks_logVM EditRow = new Base_sys_tasks_logVM();
        protected override async Task OnInitializedAsync()
        {
            await TableDataModelInit();
            await base.OnInitializedAsync();
        }
        //Model�Ի�����������¼�
        private void OnValueCallback(bool IsSuccess)
        {
            Dialog.Visible = false;
        }
        //��������ɸѡ
        int? FilterCount { get => TableDataModel.ColumnHeaderModels.Count(s => !string.IsNullOrWhiteSpace(s.search)); }
        public async Task ResetTable()
        {
            TableDataModel.ColumnHeaderModels.ForEach(s => s.search = string.Empty);
            await this.FetchPageDataAsync();
        }
        private async Task TableDataModelInit()
        {
            await _Service.InitAsync();

            this.TableDataModel = _Service.TableDataModel;
        }
        //AntDesign Table�����¼�
        async Task HandleTableChange(QueryModel<Base_sys_tasks_logVM> queryModel)
        {

            TableDataModel.FilterModel = queryModel.FilterModel;
            TableDataModel.SortModel = queryModel.SortModel;
            //var first = queryModel.SortModel.FirstOrDefault(s => s.FieldName.ToUpper() == "LOG_TIME");
            //if (first != null)
            //{
            //    TableDataModel.SortModel.Remove(first);
            //    TableDataModel.SortModel.Add(new SortModel<Base_sys_tasks_logVM>(first.ColumnIndex, first.Priority, first.FieldName, SortDirection.Descending.ToString()));
            //}
            await this.FetchPageDataAsync();

        }

        string SavedQueryModel = string.Empty; //��¼��ǰɸѡ��ֵ�������ж��Ƿ���Ҫ��̨��ѯ����
        private async Task FetchPageDataAsync()
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
                await this.FetchPageDataAsync();
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
            Dialog.Title = "�������";
            Dialog.Visible = true;
        }
        private void EditClick(Base_sys_tasks_logVM editRow)
        {
            this.IsAdd = false;
            this.EditRow = editRow.DeepClone();
            Dialog.Title = "�༭����";
            Dialog.Visible = true;
        }
        private async Task DelClickAsync(Base_sys_tasks_logVM editRow)
        {
            var confirmResult = await _confirmService.Show("ȷ��ɾ������������", "", ConfirmButtons.OKCancel);
            if (confirmResult == ConfirmResult.OK)
            {
                var ret = await _Service.RemoveRowDataAsync(editRow);
                if (ret)
                {
                    _ = _message.Success("ɾ���ɹ�");
                }
                else
                {
                    _ = _message.Error("ɾ��ʧ�ܡ�");
                }
            }
        }

        private async Task BatchDelClick()
        {
            if (TableDataModel.TableModel.SelectedRows == null || TableDataModel.TableModel.SelectedRows.Count() == 0)
            {
                _ = _message.Info("��ѡ����Ҫɾ�������ݡ�");
                return;
            }

            await _modalService.ConfirmAsync(new ConfirmOptions()
            {
                Content = $@"ȷ��ɾ����{TableDataModel.TableModel.SelectedRows.Count()}��������",
                OnOk = async (e) =>
                {
                    var ret = await _Service.RemoveRowDataAsync(TableDataModel.TableModel.SelectedRows.ToList());
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