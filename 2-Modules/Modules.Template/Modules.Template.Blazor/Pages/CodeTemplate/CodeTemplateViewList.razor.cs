using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Modules.Core.AppServices.Base_enum_data;
using Modules.Core.AppServices.ExcelServer;
using Modules.Core.AppServices.ObjectViewData;
using Modules.Core.Blazor.PageDto;
using Modules.Core.Domain.ObjectViewData;
using Modules.Core.Domain.TableModels;
using Modules.Core.Shared.Base_enum_data;
using Modules.Template.Shared.CodeTemplate;

namespace Modules.Template.Blazor.Pages.CodeTemplate
{
    partial class CodeTemplateViewList
    {
        [Parameter] public string View_id { get; set; }
        //注入服务
        [Inject] private IObjectViewDataService _ViewService { get; set; }
        [Inject] private DrawerService _DrawerService { get; set; }
        [Inject] private IBase_enum_dataService enum_DataService { get; set; }

        private List<Base_enum_dataVM> EnumDataList = new List<Base_enum_dataVM>();

        private ObjectViewDataModel ObjViewDataModel = new ObjectViewDataModel(); //这里使用无参数构造函数，为了AntTable初始化

        string SavedQueryModel = string.Empty; //记录当前筛选的值，用于判断是否需要后台查询数据

        //清除排序和筛选
        int? FilterCount { get => ObjViewDataModel.ColumnHeaderModels.Count(s => !string.IsNullOrWhiteSpace(s.search)); }
        public async Task ResetTableAsync()
        {
            ObjViewDataModel.ColumnHeaderModels.ForEach(s => s.search = string.Empty);
            await this.FetchPageDataAsync();
        }

        private Core.Domain.Base_sys_menu.CurrentMenuModel CurrentMenu = new Core.Domain.Base_sys_menu.CurrentMenuModel();
        protected override async Task OnInitializedAsync()
        {
            //取得当前用户的菜单及权限
            this.CurrentMenu = _CurrentUserService.MenuModel.GetCurrentMenu(_navigationManager.Uri);
          
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await ViewDataModelInitAsync();
                await InvokeAsync(StateHasChanged); //强制刷新
            }
        }
        private async Task ViewDataModelInitAsync()
        {
            var ret = await _ViewService.InitAsync(View_id);
            if (ret.Code == Core.Shared.ResultType.Success)
            {
                this.ObjViewDataModel = _ViewService.ObjDataModel;

                if (!ObjViewDataModel.IsValid()) _ = _message.Error("取得视图信息错误。");
                if (ObjViewDataModel.IsValid())
                {
                    this.EnumDataList = await enum_DataService.GetEnmData(ObjViewDataModel.ColumnHeaderModels);
                }
            }
            else
            {
                _ = _message.Error($@"视图 {View_id} 初始化失败。" + ret.Msg);
            }
        }

        #region 取得分页数据
        async Task HandleTableChange(QueryModel<Dictionary<string, string>> queryModel)
        {
            if (!ObjViewDataModel.IsValid())
            {
                await ViewDataModelInitAsync();
            }
            else
            {
                ObjViewDataModel.DictData.SortModel = queryModel.SortModel;
                ObjViewDataModel.DictData.FilterModel = queryModel.FilterModel;
                await FetchPageDataAsync();
            }
        }
        private async Task OnInputClearChange(string value, string field)
        {
            // 在筛选条件改变时候，才需要重新统计总数
            var searchArr = ObjViewDataModel.ColumnHeaderModels.Select(s => s.search).ToArray();
            var ThisQueryModel = string.Join(",", searchArr);

            if (SavedQueryModel == string.Empty || SavedQueryModel != ThisQueryModel)
            {
                ObjViewDataModel.DictData.PageIndex = 1; //重置到第一页
                await this.FetchPageDataAsync();
            }
        }
        private async Task FetchPageDataAsync()
        {
            if (!ObjViewDataModel.isShow)
            {
                _ = _message.Info($@"无权访问");
                this.ObjViewDataModel.DictData.Loading = false;
                await InvokeAsync(StateHasChanged);
                return;
            }
            ObjViewDataModel.DictData.Loading = true;

            try
            {
                await _ViewService.QueryPageDataAsync();
            }
            catch (Exception ex)
            {
                var confirmResult = await _confirmService.Show(ex.Message + ex.StackTrace, "分页数据查询错误", ConfirmButtons.OK, ConfirmIcon.Error);
                throw new Exception(ex.Message);
            }

            //在筛选条件改变时候，才需要重新统计总数
            var searchArr = ObjViewDataModel.ColumnHeaderModels.Select(s => s.search).ToArray();
            var ThisQueryModel = string.Join(",", searchArr);

            if (SavedQueryModel == string.Empty || SavedQueryModel != ThisQueryModel)
            {
                await Task.Run(_ViewService.QueryPageTotalCountAsync);
                //保存查询条件
                SavedQueryModel = ThisQueryModel;
            }

            ObjViewDataModel.DictData.Loading = false;
            //在线程中更改了变量状态，需手动刷新。 强制刷新
            await InvokeAsync(StateHasChanged);
            _ = _message.Success($@"当前第{ObjViewDataModel.DictData.PageIndex}页,合计{ObjViewDataModel.DictData.TotalCount}条数据。");

        }
        #endregion

        #region Excel导入导出
        [Inject] private ExportExcelObjectData excelService { get; set; }
        async Task ExportExcelClick(bool IsAllData)
        {
            if (IsAllData)
            {
                var data = await _ViewService.QueryTopPageDataAsync();
                await excelService.ExportExcelData(data, ObjViewDataModel.ColumnHeaderModels);
            }
            else
            {
                var data = await _ViewService.QueryCurrentPageDataAsync();
                await excelService.ExportExcelData(data, ObjViewDataModel.ColumnHeaderModels);
            }
        }

        //excel导入后刷新数据
        private async Task ImportDataCallbackAsync(int val)
        {
            await FetchPageDataAsync();
            ObjViewDataModel.DictData.TotalCount += val; //前端数量增加
        }
        #endregion

        #region 设置表格宽度，默认显示列等信息。
        private bool userColumnDrawerVisible = false;
        private void SettingTable()
        {
            this.userColumnDrawerVisible = true;
        }
        //用户自定义列返回事件
        private void OnUserColumnDrawerCallback(TableOptions options)
        {
            _ViewService.SetAutoColumnwidth(options.AutoColumnwidth);
            this.userColumnDrawerVisible = false;
        }
        #endregion
        #region 增删改对话框
        private readonly DialogVM Dialog = new DialogVM();
        bool IsAdd = false;
        private Dictionary<string, string> EditRow; //对话框编辑行
        //Model对话框组件返回事件
        private async Task OnAddDialogCallback(bool val)
        {
            Dialog.Visible = false;
            await this.FetchPageDataAsync();
        }
        private void AddClick()
        {
            //EditRow = new Dictionary<string, string>(); //清空前一次提交
            IsAdd = true;
            Dialog.Title = "添加数据";
            Dialog.Width = ObjViewDataModel.ColumnHeaderModels.Where(s => s.iseditable).Count() > 10 ? 1000 : 600;
            Dialog.Visible = true;
        }
        private void EditClick(Dictionary<string, string> editRow)
        {
            EditRow = editRow;
            IsAdd = false;
            Dialog.Title = "编辑数据";
            Dialog.Width = ObjViewDataModel.ColumnHeaderModels.Where(s => s.iseditable).Count() > 10 ? 1000 : 600;
            Dialog.Visible = true;
        }
        private async Task DelClick(Dictionary<string, string> Row)
        {
            if (!ObjViewDataModel.IsValid())
            {
                await ViewDataModelInitAsync();
            }
            if (!ObjViewDataModel.IsValid()) //注意：这里不能使用if-else,因为需要顺序执行
            {
                return;
            }
            var ret = false;
            await _modalService.ConfirmAsync(new ConfirmOptions()
            {

                Content = "确认删除这条数据吗？",
                OnOk = async (e) =>
                {
                    var dict = Modules.Core.Domain.Converter.DictionaryConverter.StringToObject(ObjViewDataModel.ColumnHeaderModels, Row);
                    ret = await _ViewService.RemoveRowDataAsync(dict);
                },
                OnCancel = async (e) =>
                {
                    _ = _message.Info("用户取消删除。");
                }
            });

            if (ret)
            {
                _ = _message.Success("删除成功");

                //增加动画
                Row["Row_CSS_Class"] = "Animation_DelRow";
                ObjViewDataModel.DictData.UpdateRow(Row);
                await InvokeAsync(StateHasChanged); //强制刷新
                await Task.Delay(1000);//延迟等待动画完成
                ObjViewDataModel.DictData.RemoveRow(Row);//从领域模型移除此行数据
            }
            else
            {
                _ = _message.Error("删除失败。");
            }
        }
        private void BatchDelClick()
        {
            if (ObjViewDataModel.DictData.SelectedRows == null)
            {
                _message.Info("请选择需要删除的数据。");
                return;
            }
            var count = ObjViewDataModel.DictData.SelectedRows.Count();
            if (count == 0)
            {
                _message.Info("请选择需要删除的数据。");
                return;
            }
            _modalService.ConfirmAsync(new ConfirmOptions()
            {

                Content = $@"确认删除这{count}条数据吗？",
                OnOk = async (e) =>
                {
                    var dicts = Modules.Core.Domain.Converter.DictionaryConverter.StringToObject(ObjViewDataModel.ColumnHeaderModels, ObjViewDataModel.DictData.SelectedRows.ToList());
                    var ret = await _ViewService.RemoveRowDataAsync(dicts);
                    if (ret)
                    {
                        //增加动画
                        ObjViewDataModel.DictData.SelectedRows.ForEach(row => { row["Row_CSS_Class"] = "Animation_DelRow"; });
                        await InvokeAsync(StateHasChanged); //强制刷新

                        await Task.Delay(1000); //延迟等待动画完成
                        ObjViewDataModel.DictData.RemoveRow(ObjViewDataModel.DictData.SelectedRows.ToList());//从领域模型移除此行数据

                        await InvokeAsync(StateHasChanged); //强制刷新
                        _ = _message.Success($@"删除{count}行数据成功");
                    }
                    else
                    {
                        _ = _message.Error("删除失败。");
                    }
                }
            });
        }

        private readonly DialogVM BatchEditDialog = new DialogVM();
        private void BatchEditClick()
        {
            if (ObjViewDataModel.DictData.SelectedRows == null)
            {
                _message.Info("请选择需要修改的数据。");
                return;
            }
            var count = ObjViewDataModel.DictData.SelectedRows.Count();
            if (count == 0)
            {
                _message.Info("请选择需要修改的数据。");
                return;
            }
            this.BatchEditDialog.Title = "批量修改数据";
            BatchEditDialog.Width = ObjViewDataModel.ColumnHeaderModels.Where(s => s.iseditable).Count() > 10 ? 1000 : 600;
            this.BatchEditDialog.Visible = true;
        }
        private void OnBatchDialogCallback(bool val)
        {
            BatchEditDialog.Visible = false;
        }
        #endregion

        #region 抽屉详情
        private DrawerVM DrawerDetail { get; set; } = new DrawerVM() { Width = "1100" };
        private CodeTemplateVM SelectRow = new CodeTemplateVM();//当前选中的行
        //private Expression<Func<Working_msgVM, bool>> Msg_WhereExpression; //信息反馈表达式
        private void ShowDetailDrawerClick(Dictionary<string, string> row)
        {
            this.SelectRow = _mapper.Map<CodeTemplateVM>(row);
            //信息反馈
            //Msg_WhereExpression = s => s.Project_no == SelectRow.Work_no;

            DrawerDetail.Title = "详情列表";
            DrawerDetail.Visible = true;

            //在线程中更改了变量状态，需手动刷新。 强制刷新
            //StateHasChanged();
        }
        #endregion
    }
}