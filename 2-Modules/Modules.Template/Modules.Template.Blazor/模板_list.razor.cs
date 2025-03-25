using AntDesign.TableModels;
using Modules.Core.Domain.TableDataModel;
using Modules.Template.Shared.CodeTemplate;

namespace Modules.Template.Blazor
{
    partial class 模板_list
    {
        private TableDataModel<CodeTemplateVM> TableDataModel { get; set; } = new TableDataModel<CodeTemplateVM>(); //这里使用无参数构造函数，为了AntTable初始化


        protected override async Task OnInitializedAsync()
        {
            //注意： HandleTableChange 方法会在初始化之前执行
            await base.OnInitializedAsync();
        }
        private async Task TableDataModelInit()
        {
            //注意必须初始化
            //if (string.IsNullOrWhiteSpace(ObjectId)) ObjectId = "CodeTemplate";
            //var ret = await _PageService.InitAsync(ObjectId);
            //if (ret.Code == Core.Shared.ResultType.Success)
            //{
            //    this.TableDataModel = _PageService.TableDataModel;
            //    //注入组件参数
            //    if (whereExpression != null) { TableDataModel.AttachWhereClear(); TableDataModel.AttachWhereClause(whereExpression); }
            //    //增加默认筛选条件(不能叠加)
            //    //Expression<Func<CodeTemplateVM, bool>> expression = s => s.Createuid == "test";
            //    //TableDataModel.AttachWhereClause(expression); 
            //}
            //else
            //{
            //    _= _message.Error($@"初始化失败。" + ret.Msg);
            //}
        }
        //AntDesign Table加载事件
        private async Task HandleTableChange(QueryModel<CodeTemplateVM> queryModel)
        {
            if (!TableDataModel.IsValid())
            {
                await TableDataModelInit();
            }
            else
            {
                TableDataModel.FilterModel = queryModel.FilterModel;
                TableDataModel.SortModel = queryModel.SortModel;
                //  await this.FetchPageDataAsync();
            }
        }

    }
}
