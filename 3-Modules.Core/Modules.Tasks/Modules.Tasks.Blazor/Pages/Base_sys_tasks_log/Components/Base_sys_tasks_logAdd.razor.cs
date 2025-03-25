using Force.DeepCloner;
using Microsoft.AspNetCore.Components;
using Modules.Tasks.AppServices.Base_sys_tasks_log;
using Modules.Tasks.Shared.Base_sys_tasks_log;

namespace Modules.Tasks.Blazor.Pages.Base_sys_tasks_log.Components
{
    partial class Base_sys_tasks_logAdd
    {
        //注入服务
        [Inject] private IBase_sys_tasks_logPageService _service { get; set; }
        //组件之间传值
        [Parameter] public Base_sys_tasks_logVM model { get; set; }
        [Parameter] public bool IsAdd { get; set; }
        [Parameter] public EventCallback<bool> OnValueCallback { get; set; }

        private bool btnLoading = false;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
        private async Task SubmitForm()
        {
            var ret = false;
            btnLoading = true;
            if (IsAdd)
            {
                ret = await _service.AddRowDataAsync(model.DeepClone());
            }
            else
            {
                ret = await _service.UpdateRowDataAsync(model);
            }

            if (ret)
            {
                _= _message.Success("保存成功");
            }
            else
            {
                _= _message.Error("保存失败。");
            }
            btnLoading = false;

            //给父组件传递参数
            await ReturnAsync(ret);//返回
        }
        private async Task ReturnAsync(bool IsSuccess)
        {
            //给父组件传递参数
            if (OnValueCallback.HasDelegate)
            {
                await OnValueCallback.InvokeAsync(IsSuccess);
            }
        }
    }
}