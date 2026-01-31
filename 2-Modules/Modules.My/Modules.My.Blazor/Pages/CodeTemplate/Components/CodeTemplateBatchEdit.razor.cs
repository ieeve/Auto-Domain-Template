using Microsoft.AspNetCore.Components;
using Modules.Core.AppServices.ObjectDictData;
using Modules.Core.Domain.Converter;
using Modules.Core.Domain.ObjectDictData;
using Modules.Core.Domain.TableDataModel;
using Modules.Core.Shared.ObjectDto;
using Modules.My.Shared.CodeTemplate;
using Modules.Core.Domain.DomainServices.ColumnHeader;

namespace Modules.My.Blazor.Pages.CodeTemplate.Components
{
    partial class CodeTemplateBatchEdit
    {
        [Parameter] public TableDataModel<CodeTemplateVM> TableDataModel { get; set; }
        [Parameter] public EventCallback<bool> OnValueCallback { get; set; }
        [Inject] public IDictDataService DictDataServices { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            TableDataModel.ColumnHeaderModels.ClearBindVal(); //清空上一次填写的表单信息
            await base.OnParametersSetAsync();
        }

        private bool btnLoading = false;
        private async Task SubmitForm()
        {
            List<ColumnHeaderModel> updateCol = new List<ColumnHeaderModel>();
            //为空的列不更新
            foreach (var item in TableDataModel.ColumnHeaderModels)
            {
                if (item.BatchEditEnable) updateCol.Add(item);
            }

            //生成插入数据的obj
            List<Dictionary<string, object>> dicts = new List<Dictionary<string, object>>();
            foreach (var row in TableDataModel.TableModel.SelectedRows)
            {
                //取得更新的值
                var dt = ColumnHeaderConverter.GetDictDataForColumnHeader(updateCol, TableDataModel.DbType);
                dt = DictionaryConverter.DictIgnoreCase(dt); //忽略大小写
                //增加主键
                if (dt.Keys.Contains("id")) dt["id"] = row.Id;
                else dt.Add("id", row.Id);

                dicts.Add(dt);
            }

            btnLoading = true;

            var ret = await DictDataServices.UpdateRowDataAsync(TableDataModel.TableName, "id", dicts);
            if (ret)
            {
                _ = _message.SuccessAsync("批量修改成功");
                await ReturnAsync(true);//返回
                btnLoading = false;
            }
            else
            {
                _ = _message.ErrorAsync("批量修改失败");
                btnLoading = false;
            }
        }

        private async Task ReturnAsync(bool IsSuccess)
        {
            //给父组件传递参数
            if (OnValueCallback.HasDelegate)
            {
                await OnValueCallback.InvokeAsync(IsSuccess);
            }
        }
        //解锁or锁定
        private void unlock(ColumnHeaderModel column)
        {
            column.BatchEditEnable = !column.BatchEditEnable;
        }
    }
}
