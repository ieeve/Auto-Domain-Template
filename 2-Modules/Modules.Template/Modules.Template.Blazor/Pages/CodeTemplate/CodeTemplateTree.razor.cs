using AntDesign;
using AntDesign.TableModels;
using Force.DeepCloner;
using Microsoft.AspNetCore.Components;
using Modules.Core.Blazor.PageDto;
using Modules.Core.Domain.TableModels;
using Modules.Template.AppServices.CodeTemplate;
using Modules.Template.Shared.CodeTemplate;

namespace Modules.Template.Blazor.Pages.CodeTemplate
{
    partial class CodeTemplateTree
    {
        [Inject] private ICodeTemplateTreeService _Service { get; set; }
        private TableModel<CodeTemplateTreeVM> TableModel { get; set; } = new TableModel<CodeTemplateTreeVM>();

        private AntDesign.Table<CodeTemplateTreeVM>? AntTable; //控件的@ref
        private TableOptions TableOptions { get; set; } = new TableOptions();

        //清除排序和筛选
        int? FilterCount;// { get => TableDataModel.FilterModel?.Count; }
        public void ResetTable()
        {
            AntTable?.ResetData();
        }
        private Core.Domain.Base_sys_menu.CurrentMenuModel CurrentMenu = new Core.Domain.Base_sys_menu.CurrentMenuModel();
        protected override async Task OnInitializedAsync()
        {
            //取得当前用户的菜单及权限
            this.CurrentMenu = _CurrentUserService.MenuModel.GetCurrentMenu(_navigationManager.Uri);
            await base.OnInitializedAsync();
        }
        //AntDesign Table加载事件
        async Task HandleTableChangeAsync(QueryModel<CodeTemplateTreeVM> queryModel)
        {
            //TableModel.ConditionalModels = queryModel.FilterModel.ToSqlSugarConditional();
            //TableModel.OrderByModel = queryModel.SortModel.ToSqlSugarOrderBy();
            await this.FetchPageDataAsync();
        }
        private async Task FetchPageDataAsync()
        {
            TableModel.Loading = true;
            try
            {
                var data = await _Service.QueryTreeDataAsync(TableModel); //取得TreeData数据
                TableModel.DataSource = data.Item1;
                TableModel.TotalCount = data.TotalCount;
            }
            catch (Exception ex)
            {
                var confirmResult = await _confirmService.Show(ex.Message, "执行错误", ConfirmButtons.OK, ConfirmIcon.Error);
                throw new Exception(ex.Message);
            }

            TableModel.Loading = false;

            _= _message.Success($@"合计{TableModel.TotalCount}条数据。");
        }
        #region 增删改
        private DialogVM Dialog = new DialogVM() { Width = 600 };
        private CodeTemplateTreeVM EditRow = new CodeTemplateTreeVM();
        bool IsAdd = false;
        //Model对话框组件返回事件
        private async Task OnValueCallback(CodeTemplateTreeVM IsSuccess)
        {
            Dialog.Visible = false;
            await this.FetchPageDataAsync();
        }
        private void AddClick()
        {
            this.IsAdd = true;
            Dialog.Title = "添加数据";
            Dialog.Visible = true;
        }
        private void EditClick()
        {
            if (TableModel.SelectedRows == null || TableModel.SelectedRows.Count() == 0)
            {
                _message.Info("请选择需要修改的数据。");
                return;
            }
            var editRow = TableModel.SelectedRows.LastOrDefault();
            this.IsAdd = false;
            if (editRow != null) this.EditRow = editRow.DeepClone();
            Dialog.Title = "编辑数据";
            Dialog.Visible = true;
        }
        private async Task DelClick()
        {
            if (TableModel.SelectedRows == null || TableModel.SelectedRows.Count() == 0)
            {
                _= _message.Info("请选择需要删除的数据。");
                return;
            }
            var editRow = TableModel.SelectedRows.LastOrDefault();
            var confirmResult = await _confirmService.Show("确认删除这条数据吗？", "", ConfirmButtons.OKCancel);
            if (confirmResult == ConfirmResult.OK)
            {
                var ret = await _Service.RemoveRowDataAsync(editRow);
                if (ret) _= _message.Success("删除成功"); _= _message.Success("删除失败");
            }
        }

        private async Task BatchDelClick()
        {
            if (TableModel.SelectedRows == null || TableModel.SelectedRows.Count() == 0)
            {
                _= _message.Info("请选择需要删除的数据。");
                return;
            }

            var confirmResult = await _confirmService.Show("确认删除这条数据吗？", "", ConfirmButtons.OKCancel);
            if (confirmResult == ConfirmResult.OK)
            {
                var ret = await _Service.RemoveRowDataAsync(TableModel.SelectedRows.ToList());
                if (ret) _= _message.Success("删除成功"); _= _message.Success("删除失败");
            }
        }
        #endregion
    }
}
