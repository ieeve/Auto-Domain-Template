using AntDesign;
using AntDesign.TableModels;
using Force.DeepCloner;
using Microsoft.AspNetCore.Components;
using Modules.CodeGenerator.Template.AppServices.CodeTemplate;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;
using Modules.Core.Blazor.PageDto;
using Modules.Core.Domain.DomainServices.AntDesignExt;
using Modules.Core.Domain.TableDataModel;
using Modules.Core.Domain.TableModels;
using Modules.Core.Shared.Base_object_extend;
using SqlSugar;
using System.Text.Json;

namespace Modules.CodeGenerator.Template.Blazor.Pages.CodeTemplate.Components
{
    partial class CodeTemplateSimpleListComp
    {
        [Parameter] public List<ConditionalModel> WhereConditional { get; set; }
        [Parameter] public CodeTemplateVM ModalVM { get; set; } = new CodeTemplateVM(); //用于父组件预设的数据，比如父ID
        [Parameter] public TableOptions TableOptions { get; set; } = new TableOptions(); //注意必须初始化
        [Inject] private ICodeTemplateSimplePageService _Service { get; set; }
        private TableModel<CodeTemplateVM> TableModel { get; set; } = new TableModel<CodeTemplateVM>();
        private AntDesign.Table<CodeTemplateVM>? AntTable; //控件的@ref

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
            await base.OnInitializedAsync();
        }

        private string Json_WhereExpression = string.Empty;

        //添加方式
        //TableModel.ConditionalModels.Add(new ConditionalModel { FieldName = "Msg_id", ConditionalType = ConditionalType.Equal, FieldValue = this.Msg_id });
        protected override async Task OnParametersSetAsync()
        {
            //注意：这里会被多次调用 * 在调用页面初始化WhereConditional=null,在HandleTableChangeAsync调用时候避免首次加载
            if (WhereConditional != null && Json_WhereExpression != JsonSerializer.Serialize(WhereConditional))
            {
                this.Json_WhereExpression = JsonSerializer.Serialize(WhereConditional);
                //添加筛选参数
                TableModel.ConditionalModels.Clear();
                //增加默认排序条件(不能叠加);
                //TableDataModel.SortModel.Add(new SortModel<Base_sys_tasks_logVM>(first.ColumnIndex, first.Priority, first.FieldName, SortDirection.Descending.ToString()));
                AntTable?.ReloadData();
                //await InvokeAsync(StateHasChanged); //如果进行了异步操作，需要强制刷新
            }
            await base.OnParametersSetAsync();
        }

        //AntDesign Table加载事件
        async Task HandleTableChangeAsync(QueryModel<CodeTemplateVM> queryModel)
        {
            if (WhereConditional != null)
            {
                TableModel.ConditionalModels = queryModel.FilterModel.ToSqlSugarConditional();
                TableModel.ConditionalModels.AddRange(WhereConditional);
                TableModel.OrderByModel = queryModel.SortModel.ToSqlSugarOrderBy();
                ////添加默认排序
                //if (TableModel.OrderByModel.Count == 0)
                //{
                //    TableModel.OrderByModel.Add(new OrderByModel { FieldName = "Seqno", OrderByType = OrderByType.Asc });
                //}
                await this.FetchPageDataAsync();
            }
        }
        private async Task FetchPageDataAsync()
        {
            TableModel.Loading = true;
            var data = await _Service.GetSimplePageData(TableModel);
            TableModel.DataSource = data.Item1;
            TableModel.TotalCount = data.TotalCount;
            TableModel.Loading = false;
            _ = _message.SuccessAsync($@"当前第{TableModel.PageIndex}页,合计{TableModel.TotalCount}条数据。");
        }

        #region 增删改
        private DialogVM Dialog = new DialogVM();
        bool IsAdd = false;
        private CodeTemplateVM EditRow = new CodeTemplateVM();
        //Model对话框组件返回事件
        private void OnValueCallback(CodeTemplateVM row)
        {
            Dialog.Visible = false;
            if (IsAdd) this.TableModel.AddRow(row);
            else this.TableModel.UpdateRow(row);
        }
        private void AddClick()
        {
            this.IsAdd = true;
            this.EditRow = ModalVM; //用于父组件预设的数据，比如父ID
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
                    this.TableModel.RemoveRow(editRow);
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
                    TableModel.RemoveRow(TableModel.SelectedRows);
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
