using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Modules.Core.Blazor.PageDto;
using Modules.Core.Domain.DomainServices.AntDesignExt;
using Modules.Core.Domain.TableModels;
using Modules.Template.AppServices.CodeTemplate;
using Modules.Template.Shared.CodeTemplate;
using SqlSugar;
using System.Text.Json;

namespace Modules.Template.Blazor.Pages.CodeTemplate.Components
{
    partial class CodeTemplateSimpleListComp
    {
        [Parameter] public List<ConditionalModel> WhereConditional { get; set; }
        [Parameter] public TableOptions TableOptions { get; set; } = new TableOptions(); //注意必须初始化
        [Inject] private ICodeTemplateSimplePageService _Service { get; set; }
        private TableModel<CodeTemplateVM> TableModel { get; set; } = new TableModel<CodeTemplateVM>();
        private AntDesign.Table<CodeTemplateVM>? AntTable; //控件的@ref

        //清除排序和筛选
        public void ResetTable()
        {
            AntTable?.ResetData();
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
                AntTable?.ReloadData();
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
            //在线程中更改了变量状态，需手动刷新。(似乎是不需要了)
            //await InvokeAsync(StateHasChanged);
            _ = _message.Success($@"当前第{TableModel.PageIndex}页,合计{TableModel.TotalCount}条数据。");
        }

        #region 增删改
        private DialogVM Dialog = new DialogVM();
        private CodeTemplateVM EditRow = new CodeTemplateVM();
        bool IsAdd = false;
        //Model对话框组件返回事件
        private void OnValueCallback(CodeTemplateVM row)
        {
            Dialog.Visible = false;
            if (row != null)
            {
                var index = this.TableModel.DataSource.FindIndex(s => s.Id == row.Id);
                if (index >= 0)
                    TableModel.DataSource[index] = row;
                else
                     if (TableModel.DataSource.Count > 0) TableModel.DataSource.Insert(0, row); else TableModel.DataSource.Add(row);
            }
        }
        private void AddClick()
        {
            this.IsAdd = true;
            Dialog.Title = "添加数据";
            Dialog.Visible = true;
        }
        private void EditClick(CodeTemplateVM editRow)
        {
            this.IsAdd = false;
            this.EditRow = editRow;
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
                    TableModel.DataSource.Remove(editRow);
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
            if (TableModel.SelectedRows == null || TableModel.SelectedRows.Count() == 0)
            {
                _ = _message.Info("请选择需要删除的数据。");
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
                        TableModel.DataSource.Remove(row);
                    }

                    _ = _message.Success("删除成功");
                }
                else
                {
                    _ = _message.Error("删除失败。");
                }
            }
        }
        #endregion
    }
}
