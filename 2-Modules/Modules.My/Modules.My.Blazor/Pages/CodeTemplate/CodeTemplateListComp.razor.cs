using AntDesign;
using AntDesign.TableModels;
using Force.DeepCloner;
using Microsoft.AspNetCore.Components;
using Modules.My.AppServices.CodeTemplate;
using Modules.My.Shared.CodeTemplate;
using Modules.Core.AppServices.ExcelServer;
using Modules.Core.Blazor.Base;
using Modules.Core.Blazor.PageDto;
using Modules.Core.Domain.TableDataModel;
using Modules.Core.Domain.TableModels;
using System.Linq.Expressions;

namespace Modules.My.Blazor.Pages.CodeTemplate
{
    partial class CodeTemplateListComp: BaseObjectPage
    {
        [Parameter] public Expression<Func<CodeTemplateVM, bool>> WhereExpression { get; set; }

        //注入服务
        [Inject] private ICodeTemplatePageService _Service { get; set; }
        [Inject] private IMessageService _message { get; set; }
        [Inject] private ModalService _modalService { get; set; }
        [Inject] private IConfirmService _confirmService { get; set; }

        private DialogVM Dialog = new DialogVM();
        private TableDataModel<CodeTemplateVM> TableDataModel { get; set; } = new TableDataModel<CodeTemplateVM>(); //这里使用无参数构造函数，为了AntTable初始化
        private Table<CodeTemplateVM>? AntTable; //控件的@ref 
        bool IsAdd = false;
        private CodeTemplateVM EditRow = new CodeTemplateVM();
        //Model对话框组件返回事件
        private void OnValueCallback(CodeTemplateVM IsSuccess)
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

        private Core.Domain.Base_sys_menu.CurrentMenuModel CurrentMenu = new Core.Domain.Base_sys_menu.CurrentMenuModel();
        protected override async Task OnInitializedAsync()
        {
            //取得当前用户的菜单及权限
            this.CurrentMenu = _CurrentUserService.MenuModel.GetCurrentMenu(_navigationManager.Uri);
            await base.OnInitializedAsync();
        }

        private Expression<Func<CodeTemplateVM, bool>> Old_WhereExpression;
        protected override async Task OnParametersSetAsync()
        {
            //注意：这里会被多次调用
            if (WhereExpression != null && Old_WhereExpression != WhereExpression)
            {
                this.Old_WhereExpression = WhereExpression;

                if (!TableDataModel.IsValid())
                {
                    await TableDataModelInit();
                }
                TableDataModel.AttachWhereClear();
                TableDataModel.AttachWhereClause(WhereExpression);
                await this.FetchPageDataAsync(true);
            }
            if (WhereExpression == null && Old_WhereExpression != null)
            {
                TableDataModel.AttachWhereClear();
                await this.FetchPageDataAsync(true);
            }
            await base.OnParametersSetAsync();
        }
        private async Task TableDataModelInit()
        {
            await _Service.InitAsync();

            this.TableDataModel = _Service.TableDataModel;
        }
        //AntDesign Table加载事件
        private async Task HandleTableChange(QueryModel<CodeTemplateVM> queryModel)
        {
            //在Razor页面已经验证过TableDataModel.IsValid()了
            TableDataModel.FilterModel = queryModel.FilterModel;
            TableDataModel.SortModel = queryModel.SortModel;
            await this.FetchPageDataAsync();
        }

        string SavedQueryModel = string.Empty; //记录当前筛选的值，用于判断是否需要后台查询数据
        private async Task FetchPageDataAsync(bool QueryCount = false)
        {
            TableDataModel.TableModel.Loading = true;
            try
            {
                await _Service.QueryPageDataAsync();
            }
            catch (Exception ex)
            {
                var confirmResult = await _confirmService.Show(ex.StackTrace, ex.Message, ConfirmButtons.OK, ConfirmIcon.Error);
            }
            //在筛选条件改变时候，才需要重新统计总数
            var searchArr = TableDataModel.ColumnHeaderModels.Select(s => s.search).ToArray();
            var ThisQueryModel = string.Join(",", searchArr);

            if (SavedQueryModel == string.Empty || SavedQueryModel != ThisQueryModel || QueryCount)
            {
                //await Task.Run(_Service.QueryPageTotalCountAsync);
                await _Service.QueryPageTotalCountAsync();
                //保存查询条件
                SavedQueryModel = ThisQueryModel;
            }
            TableDataModel.TableModel.Loading = false;
            _ = _message.SuccessAsync($@"当前第{TableDataModel.TableModel.PageIndex}页,合计{TableDataModel.TableModel.TotalCount}条数据。");

        }
        private async Task OnInputClearChange(string value, string field)
        {
            // 在筛选条件改变时候，才需要重新统计总数
            var searchArr = TableDataModel.ColumnHeaderModels.Select(s => s.search).ToArray();
            var ThisQueryModel = string.Join(",", searchArr);

            if (SavedQueryModel == string.Empty || SavedQueryModel != ThisQueryModel)
            {
                TableDataModel.TableModel.PageIndex = 1; //重置到第一页
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
        private async Task OnUserColumnDrawerCallbackAsync(TableOptions tableOptions)
        {
            TableDataModel.TableOptions = tableOptions;
            this.userColumnDrawerVisible = false;
            await this.FetchPageDataAsync();
        }
        #endregion

        #region 增删改
        private void AddClick()
        {
            this.IsAdd = true;
            this.EditRow = new CodeTemplateVM();
            Dialog.Title = "新增";
            Dialog.Visible = true;
        }
        private void EditClick(CodeTemplateVM editRow)
        {
            this.IsAdd = false;
            this.EditRow = editRow.DeepClone();
            Dialog.Title = "编辑";
            Dialog.Visible = true;
        }
        private async Task DelClickAsync(CodeTemplateVM editRow)
        {
            var confirmResult = await _confirmService.Show("确认删除这条数据吗？", "", ConfirmButtons.OKCancel);
            if (confirmResult == ConfirmResult.OK)
            {
                var ret = await _Service.RemoveRowDataAsync(editRow);
                if (ret)
                {
                    _ = _message.SuccessAsync("删除成功");
                }
                else
                {
                    _ = _message.ErrorAsync("删除失败。");
                }
            }
        }

        private async Task BatchDelClick()
        {
            if (TableDataModel.TableModel.SelectedRows == null || TableDataModel.TableModel.SelectedRows.Count() == 0)
            {
                _ = _message.InfoAsync("请选择需要删除的数据。");
                return;
            }

            var ret = false;
            await _modalService.ConfirmAsync(new ConfirmOptions()
            {

                Content = $@"确认删除这{TableDataModel.TableModel.SelectedRows.Count()}条数据吗？",
                OnOk = async (e) =>
                {
                    ret = await _Service.RemoveRowDataAsync(TableDataModel.TableModel.SelectedRows.ToList());
                    if (ret)
                    {
                        _ = _message.SuccessAsync("删除成功");
                    }
                    else
                    {
                        _ = _message.ErrorAsync("删除失败。");
                    }
                },
                OnCancel = async (e) =>
                {
                    _ = _message.InfoAsync("用户取消删除。");
                }
            });
        }
        #endregion
    }
}