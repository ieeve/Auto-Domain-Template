using AntDesign;
using AntDesign.TableModels;
using Force.DeepCloner;
using Microsoft.AspNetCore.Components;
using Modules.Core.Blazor.PageDto;
using Modules.Core.Domain.DomainServices.AntDesignExt;
using Modules.Core.Domain.TableModels;
using Modules.CodeGenerator.Template.AppServices.CodeTemplate;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;
using SqlSugar;

namespace Modules.CodeGenerator.Template.Blazor.Pages.CodeTemplate
{
    partial class CodeTemplateSimpleList
    {
        [Inject] private ICodeTemplateSimplePageService _Service { get; set; }
        private TableModel<CodeTemplateVM> TableModel { get; set; } = new TableModel<CodeTemplateVM>();
        private AntDesign.Table<CodeTemplateVM>? AntTable; //控件的@ref
        private TableOptions TableOptions { get; set; } = new TableOptions();

        //清除排序和筛选
        public void ResetTable()
        {
            AntTable?.ResetData();
        }

        private Core.Domain.Base_sys_menu.CurrentMenuModel CurrentMenu = new Core.Domain.Base_sys_menu.CurrentMenuModel();
        protected override async Task OnInitializedAsync()
        {
            //取得当前用户的菜单及权限
            this.CurrentMenu = _CurrentUserService.MenuModel.GetCurrentMenu(_navigationManager.Uri);
            //增加默认筛选条件(不能叠加)
            //TableModel.ConditionalModels.Clear();
            //TableModel.ConditionalModels.Add(new ConditionalModel { FieldName = "Module_id", ConditionalType = ConditionalType.Equal, FieldValue = Module_id });
            await base.OnInitializedAsync();
        }

        //AntDesign Table加载事件
        async Task HandleTableChangeAsync(QueryModel<CodeTemplateVM> queryModel)
        {
            TableModel.ConditionalModels = queryModel.FilterModel.ToSqlSugarConditional();
            TableModel.OrderByModel = queryModel.SortModel.ToSqlSugarOrderBy();
            await this.FetchPageDataAsync();
        }
        private async Task FetchPageDataAsync()
        {
            TableModel.Loading = true;
            var data = await _Service.GetSimplePageData(TableModel);
            TableModel.DataSource = data.Item1;
            TableModel.TotalCount = data.TotalCount;
            TableModel.Loading = false;
            
            //await InvokeAsync(StateHasChanged);
            _ = _message.SuccessAsync($@"当前第{TableModel.PageIndex}页,合计{TableModel.TotalCount}条数据。");
        }

        #region 增删改
        private DialogVM Dialog = new DialogVM();
        private CodeTemplateVM EditRow = new CodeTemplateVM();
        bool IsAdd = false;
        //Model对话框组件返回事件
        private void OnValueCallback(CodeTemplateVM row)
        {
            if (IsAdd) this.TableModel.AddRow(row);
            else this.TableModel.UpdateRow(row);
            Dialog.Visible = false;
        }
        private void AddClick()
        {
            this.IsAdd = true;
            this.EditRow = new CodeTemplateVM();
            Dialog.Title = "添加数据";
            Dialog.Visible = true;
        }
        private void EditClick(CodeTemplateVM editRow)
        {
            this.IsAdd = false;
            this.EditRow = editRow.DeepClone(); //深拷贝一份，防止取消时，前端数据被修改
            Dialog.Title = "编辑数据";
            Dialog.Visible = true;
        }
        private async Task DelClick(CodeTemplateVM editRow)
        {
            var confirmResult = await _confirmService.Show("确认删除这条数据吗？", "", ConfirmButtons.OKCancel);
            if (confirmResult == ConfirmResult.OK)
            {
                var ret = await _Service.RemoveRowDataAsync(editRow);
                if (ret)
                {
                    TableModel.RemoveRow(editRow);
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
            if (TableModel.SelectedRows == null || TableModel.SelectedRows.Count() == 0)
            {
                _ = _message.InfoAsync("请选择需要删除的数据。");
                return;
            }

            var confirmResult = await _confirmService.Show("确认删除这条数据吗？", "", ConfirmButtons.OKCancel);
            if (confirmResult == ConfirmResult.OK)
            {
                var ret = await _Service.RemoveRowDataAsync(TableModel.SelectedRows.ToList());
                if (ret)
                {
                    foreach (var row in TableModel.SelectedRows)
                    {
                        TableModel.RemoveRow(row);
                    }

                    _ = _message.SuccessAsync("删除成功");
                }
                else
                {
                    _ = _message.ErrorAsync("删除失败。");
                }
            }
        }
        #endregion
    }
}
