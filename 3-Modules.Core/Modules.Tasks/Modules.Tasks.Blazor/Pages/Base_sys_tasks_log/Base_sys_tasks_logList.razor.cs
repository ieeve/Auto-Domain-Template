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
        //注入服务
        [Inject] private IBase_sys_tasks_logPageService _Service { get; set; }

        private DialogVM Dialog = new DialogVM();
        private TableDataModel<Base_sys_tasks_logVM> TableDataModel { get; set; } = new TableDataModel<Base_sys_tasks_logVM>(); //这里使用无参数构造函数，为了AntTable初始化

        bool IsAdd = false;
        private string PageTitleString = "系统任务日志";
        private Base_sys_tasks_logVM EditRow = new Base_sys_tasks_logVM();
        protected override async Task OnInitializedAsync()
        {
            await TableDataModelInit();
            await base.OnInitializedAsync();
        }
        //Model对话框组件返回事件
        private void OnValueCallback(bool IsSuccess)
        {
            Dialog.Visible = false;
        }
        //清除排序和筛选
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
        //AntDesign Table加载事件
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

        string SavedQueryModel = string.Empty; //记录当前筛选的值，用于判断是否需要后台查询数据
        private async Task FetchPageDataAsync()
        {
            TableDataModel.TableModel.Loading = true;


            try
            {
                await _Service.QueryPageDataAsync(); //取得分页数据
            }
            catch (Exception ex)
            {
                var confirmResult = await _confirmService.Show(ex.StackTrace, ex.Message, ConfirmButtons.OK, ConfirmIcon.Error);
            }
            //在筛选条件改变时候，才需要重新统计总数
            var searchArr = TableDataModel.ColumnHeaderModels.Select(s => s.search).ToArray();
            var ThisQueryModel = string.Join(",", searchArr);

            if (SavedQueryModel == string.Empty || SavedQueryModel != ThisQueryModel)
            {
                await _Service.QueryPageTotalCountAsync();
                //保存查询条件
                SavedQueryModel = ThisQueryModel;
            }
            TableDataModel.TableModel.Loading = false;


            //在线程中更改了变量状态，需手动刷新。
            // await InvokeAsync(StateHasChanged);
            _ = _message.Success($@"当前第{TableDataModel.TableModel.PageIndex}页,合计{TableDataModel.TableModel.TotalCount}条数据。");

        }
        private async Task OnInputClearChange(string value, string field)
        {
            // 在筛选条件改变时候，才需要重新统计总数
            var searchArr = TableDataModel.ColumnHeaderModels.Select(s => s.search).ToArray();
            var ThisQueryModel = string.Join(",", searchArr);

            if (SavedQueryModel == string.Empty || SavedQueryModel != ThisQueryModel)
            {
                await this.FetchPageDataAsync();
            }
        }

        #region 导出excel
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
        #region 设置表格宽度，默认显示列等信息。
        private bool userColumnDrawerVisible = false;
        private void SettingTable()
        {
            this.userColumnDrawerVisible = true;
        }
        //用户自定义列返回事件
        private void OnUserColumnDrawerCallback()
        {
            this.userColumnDrawerVisible = false;
        }
        #endregion

        #region 增删改
        private void AddClick()
        {
            this.IsAdd = true;
            Dialog.Title = "添加数据";
            Dialog.Visible = true;
        }
        private void EditClick(Base_sys_tasks_logVM editRow)
        {
            this.IsAdd = false;
            this.EditRow = editRow.DeepClone();
            Dialog.Title = "编辑数据";
            Dialog.Visible = true;
        }
        private async Task DelClickAsync(Base_sys_tasks_logVM editRow)
        {
            var confirmResult = await _confirmService.Show("确认删除这条数据吗？", "", ConfirmButtons.OKCancel);
            if (confirmResult == ConfirmResult.OK)
            {
                var ret = await _Service.RemoveRowDataAsync(editRow);
                if (ret)
                {
                    _ = _message.Success("删除成功");
                }
                else
                {
                    _ = _message.Error("删除失败。");
                }
            }
        }

        private async Task BatchDelClick()
        {
            if (TableDataModel.TableModel.SelectedRows == null || TableDataModel.TableModel.SelectedRows.Count() == 0)
            {
                _ = _message.Info("请选择需要删除的数据。");
                return;
            }

            await _modalService.ConfirmAsync(new ConfirmOptions()
            {
                Content = $@"确认删除这{TableDataModel.TableModel.SelectedRows.Count()}条数据吗？",
                OnOk = async (e) =>
                {
                    var ret = await _Service.RemoveRowDataAsync(TableDataModel.TableModel.SelectedRows.ToList());
                    if (ret)
                    {
                        await InvokeAsync(StateHasChanged); //强制刷新
                        _ = _message.Success("删除成功");
                    }
                    else
                    {
                        _ = _message.Error("删除失败。");
                    }
                },
                OnCancel = async (e) =>
                {
                    _ = _message.Info("用户取消删除。");
                }
            });
        }
        #endregion
    }
}