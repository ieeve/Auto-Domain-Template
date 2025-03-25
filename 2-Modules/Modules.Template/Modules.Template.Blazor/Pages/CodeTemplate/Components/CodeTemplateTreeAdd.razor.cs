using AntDesign;
using Force.DeepCloner;
using Microsoft.AspNetCore.Components;
using Modules.Template.AppServices.CodeTemplate;
using Modules.Template.Shared.CodeTemplate;

namespace Modules.Template.Blazor.Pages.CodeTemplate.Components
{
    partial class CodeTemplateTreeAdd
    {
        //注入服务
        [Inject] private ICodeTemplateTreeService _service { get; set; }
        [Inject] private IMessageService _message { get; set; }
        //组件之间传值
        [Parameter] public CodeTemplateTreeVM model { get; set; }
        [Parameter] public bool IsAdd { get; set; }
        [Parameter] public EventCallback<CodeTemplateTreeVM> OnValueCallback { get; set; }

        private bool btnLoading = false;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
        private async Task SubmitForm()
        {
            btnLoading = true;
            if (IsAdd)
            {
                var ret = await _service.AddRowDataAsync(model.DeepClone());
                if (ret != null) _= _message.Success("保存成功"); _= _message.Success("保存失败");
            }
            else
            {
                var ret = await _service.UpdateRowDataAsync(model);
                if (ret) _= _message.Success("保存成功"); _= _message.Success("保存失败");
            }
            //给父组件传递参数
            await ReturnAsync(model);//返回
            btnLoading = false;
        }
        private async Task ReturnAsync(CodeTemplateTreeVM VM)
        {
            //给父组件传递参数
            if (OnValueCallback.HasDelegate)
            {
                await OnValueCallback.InvokeAsync(VM);
            }
        }
    }
}