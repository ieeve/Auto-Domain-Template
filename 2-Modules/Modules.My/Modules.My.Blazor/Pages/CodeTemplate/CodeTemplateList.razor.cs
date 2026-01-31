using AntDesign;
using AntDesign.TableModels;
using Force.DeepCloner;
using Microsoft.AspNetCore.Components;
using Modules.My.AppServices.CodeTemplate;
using Modules.My.Shared.CodeTemplate;
using Modules.Core.AppServices.ExcelServer;
using Modules.Core.Blazor.PageDto;
using Modules.Core.Domain.TableDataModel;
using Modules.Core.Domain.TableModels;
using System.Linq.Expressions;

namespace Modules.My.Blazor.Pages.CodeTemplate
{
    partial class CodeTemplateList
    {
        [Inject] private ICodeTemplatePageService _PageService { get; set; }
        private TableDataModel<CodeTemplateVM> TableDataModel { get; set; } = new TableDataModel<CodeTemplateVM>(); //这里使用无参数构造函数，为了AntTable初始化
        private Core.Domain.Base_sys_menu.CurrentMenuModel CurrentMenu = new Core.Domain.Base_sys_menu.CurrentMenuModel();
        protected override async Task OnInitializedAsync()
        {
            //取得当前用户的菜单及权限
            this.CurrentMenu = _CurrentUserService.MenuModel.GetCurrentMenu(_navigationManager.Uri);
            await TableDataModelInit();
            await base.OnInitializedAsync();
        }
        private async Task TableDataModelInit()
        {
            await _PageService.InitAsync();
            this.TableDataModel = _PageService.TableDataModel;
            //增加默认筛选条件(不能叠加)
            //Expression<Func<CodeTemplateVM, bool>> expression = s => s.Createuid == "test";
            //TableDataModel.AttachWhereClause(expression); 

            //添加默认排序
            //this.TableDataModel.AopOrderBy = "create_time desc";
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
        private AntDesign.Table<CodeTemplateVM>? AntTable; //控件的@ref
        //清除排序和筛选
        int? FilterCount { get => TableDataModel.ColumnHeaderModels.Count(s => !string.IsNullOrWhiteSpace(s.search)); }
        public async Task ResetTable()
        {
            TableDataModel.ColumnHeaderModels.ForEach(s => s.search = string.Empty);
            await this.FetchPageDataAsync();
        }
        private async Task FetchPageDataAsync(bool QueryCount = false)
        {
            TableDataModel.TableModel.Loading = true;
            try
            {
                await _PageService.QueryPageDataAsync();
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
                await _PageService.QueryPageTotalCountAsync();
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

        #region 导入导出excel
        [Inject] private ExportExcelObjectData excelService { get; set; }
        private async Task ImportDataCallbackAsync(int val)
        {
            await FetchPageDataAsync();
            TableDataModel.TableModel.TotalCount += val; //前端数量增加
        }
        async Task ExportExcelClick(bool IsAllData)
        {
            string key = $"updatable-{DateTime.Now.Ticks}";
            var config = new MessageConfig()
            {
                Content = "正在导出,请稍后...",
                Key = key
            };
            if (IsAllData)
            {
                var data = await _PageService.QueryTopPageDataAsync();
                await excelService.ExportExcelData(data, TableDataModel.ColumnHeaderModels);
            }
            else
            {
                var data = await _PageService.QueryCurrentPageDataAsync();
                await excelService.ExportExcelData(data, TableDataModel.ColumnHeaderModels);
            }
            config.Content = "导出完毕。";
            config.Duration = 2;
            await _message.SuccessAsync(config);
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
        bool IsAdd = false;
        private DialogVM Dialog = new DialogVM();
        private CodeTemplateVM EditRow = new CodeTemplateVM();
        //Model对话框组件返回事件
        private void OnValueCallback(CodeTemplateVM row)
        {
            Dialog.Visible = false;
            if (IsAdd) this.TableDataModel.TableModel.AddRow(row);
            else this.TableDataModel.TableModel.UpdateRow(row);
        }
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
            this.EditRow = editRow.DeepClone(); //深拷贝一份，防止取消时，前端数据被修改
            Dialog.Title = "编辑";
            Dialog.Visible = true;
        }
        private async Task DelClickAsync(CodeTemplateVM editRow)
        {
            var confirmResult = await _confirmService.Show("确认删除这条数据吗？", "", ConfirmButtons.OKCancel);
            if (confirmResult == ConfirmResult.OK)
            {
                var ret = await _PageService.RemoveRowDataAsync(editRow);
                if (ret)
                {
                    this.TableDataModel.TableModel.RemoveRow(editRow);
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

            await _modalService.ConfirmAsync(new ConfirmOptions()
            {

                Content = $@"确认删除这{TableDataModel.TableModel.SelectedRows.Count()}条数据吗？",
                OnOk = async (e) =>
                {
                    var ret = await _PageService.RemoveRowDataAsync(TableDataModel.TableModel.SelectedRows.ToList());
                    if (ret)
                    {
                        _ = _message.SuccessAsync("删除成功");
                        this.TableDataModel.TableModel.RemoveRow(TableDataModel.TableModel.SelectedRows);
                        await InvokeAsync(StateHasChanged); //强制刷新
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

        private readonly DialogVM BatchEditDialog = new DialogVM();
        private void BatchEditClick()
        {
            if (TableDataModel.TableModel.SelectedRows == null || TableDataModel.TableModel.SelectedRows.Count() == 0)
            {
                _message.Info("请选择需要修改的数据。");
                return;
            }
            this.BatchEditDialog.Title = "批量修改数据";
            BatchEditDialog.Width = TableDataModel.ColumnHeaderModels.Where(s => s.iseditable).Count() > 10 ? 1000 : 600;
            this.BatchEditDialog.Visible = true;
        }
        private async Task OnBatchDialogCallback(bool success)
        {
            BatchEditDialog.Visible = false;
            if (success) await _PageService.QueryPageDataAsync(); 
        }
        #endregion

        #region 显示行详情
        private bool RowDetailVisible = false;
        private Dictionary<string, string> ShowDetailRow = new Dictionary<string, string>();
        private void ShowClick(CodeTemplateVM editRow)
        {
            ShowDetailRow = Core.Domain.Converter.DictionaryConverter.ModelToDictionary2(editRow); ;
            RowDetailVisible = true;
        }
        #endregion
    }
}