using Force.DeepCloner;
using Microsoft.AspNetCore.Components;
using Modules.Tasks.AppServices.Base_sys_tasks_log;
using Modules.Tasks.Shared.Base_sys_tasks_log;

namespace Modules.Tasks.Blazor.Pages.Base_sys_tasks_log.Components
{
    partial class Base_sys_tasks_logAdd
    {
        //ע�����
        [Inject] private IBase_sys_tasks_logPageService _service { get; set; }
        //���֮�䴫ֵ
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
                _= _message.Success("����ɹ�");
            }
            else
            {
                _= _message.Error("����ʧ�ܡ�");
            }
            btnLoading = false;

            //����������ݲ���
            await ReturnAsync(ret);//����
        }
        private async Task ReturnAsync(bool IsSuccess)
        {
            //����������ݲ���
            if (OnValueCallback.HasDelegate)
            {
                await OnValueCallback.InvokeAsync(IsSuccess);
            }
        }
    }
}