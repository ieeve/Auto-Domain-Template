using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Modules.Core.AppServices.Authentication;
using Modules.My.AppServices.CodeTemplate;

namespace Modules.My.Blazor
{
    /*注意命名空间一定要和razor文件的目录空间一致*/
    public partial class 模板_page : ComponentBase
    {
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] protected IMessageService _message { get; set; }
        [Inject] protected IJSRuntime _jsRuntime { get; set; }
        [Parameter] public string Id { get; set; } = string.Empty;
        //注入
        [Inject] private ICodeTemplateService _Services { get; set; }
        [Inject] protected ICurrentUserService _userService { get; set; }
        /*
         * 仔细阅读微软文档
         * https://learn.microsoft.com/zh-cn/aspnet/core/blazor/components/lifecycle?view=aspnetcore-8.0
         * 
         SetParametersAsync，设置参数之前接受到参数（获取URL参数）。在呈现树中设置组件的父组件提供的参数
         OnInitializedAsync，初始化之后（在页面渲染前获得数据）。
         OnParametersSetAsync，设置完参数之后（该事件可以多次触发, 参数发生了改变也会再次触发）。
         OnAfterRenderAsync，组件呈现之后（组件呈现之后触发）。
         

        //除非自定义基类与自定义逻辑一起使用，否则不需要调用 base.OnInitializedAsync。
          * 服务器上【预呈现】其内容 会调用 OnInitializedAsync 两次：
        
         其他
Dispose 移除组件时触发，通常在该函数里面进行一些特定资源的释放。

ShouldRender 如果需要阻止UI刷新，可以覆写该方法。

StateHasChanged 通知UI，状态已经更改，以便UI适时刷新。什么时候我们需要调用StateHasChange()
        1. 在一个异步方法中，我们多次进入异步状态 接收外部的事件，处理完之后 
        2. 在渲染树外部进行渲染(例如javascript调用，由javascript直接和Dom操作）
         */
        /*
         ParameterView包含了所有的参数，主要有URL里面的参数以及父组件通过属性传递过来的参数，
         可以通过foreach遍历里面的值，也可以通过GetValueOrDefault方法根据参数名取值
         */
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            //parameters.SetParameterProperties(this); //保存参数
            //此时不能获取变量值，需要通过参数获取
            //var object_id = parameters.GetValueOrDefault<string>("object_id");

            //例子：
            //验证组件必备参数是否为空,
            List<string> ErrorMsg = new List<string>();
            foreach (ParameterValue p in parameters)
            {
                if (p.Value == null) ErrorMsg.Add($@"***：参数 {p.Name} 的值不能为空。");
            }

            if (ErrorMsg.Count > 0)
            {
                ErrorMsg.ForEach(async x =>
                {
                    _message.Error(x);
                });
            }
            else
            {
                await base.SetParametersAsync(parameters);
            }
        }



        protected override async Task OnInitializedAsync()
        {
            var task = Task.Run(async () =>
            {
                //this.TreeData = await _Service.QueryDataTreeAsync(); //取得role树
            });
            task.Wait(); //如果后面需要赋值，这里要等待完成
                         //FetchData(); 获取数据，
                         //除非自定义基类与自定义逻辑一起使用，否则不需要调用 base.OnInitializedAsync。
                         // await base.OnInitializedAsync();
        }

       private string Old_id;

        //*注意：当前类要继承ComponentBase
        protected override async Task OnParametersSetAsync()
        {
                //** 这里异步 如果Mysql数据库需要CopyNew()强制new出新对象
                if (!string.IsNullOrWhiteSpace(Id) && this.Id != Old_id)
                {
                    this.Old_id = Id;

                // this.TreeData = await _Service.QueryDataTreeAsync(TenantId); //取得role树

                //StateHasChanged();
                await InvokeAsync(StateHasChanged); //如果进行了异步操作，需要强制刷新
                }

            //除非自定义基类与自定义逻辑一起使用，否则不需要调用 base.OnInitializedAsync。
            //await base.OnParametersSetAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //组件呈现之后触发，通常用来做JavaScript的互操作，需要注意的是要加上对firstRender参数的判断
            if (firstRender)
            {
                string url = "www.baidu.com";
                await _jsRuntime.InvokeAsync<object>("open", url, "_blank");
                await InvokeAsync(StateHasChanged); //如果进行了异步操作，需要强制刷新。
            }
            await base.OnAfterRenderAsync(firstRender);
        }


        bool loading = false;
        private void enterLoading()
        {
            loading = true;
            Task.Run(async () =>
            {
                await Task.Delay(3000);
                loading = false;
                //在线程中更改了变量状态，需手动刷新。 强制刷新
                //注意使用如下方式显示提示信息
                _ = _message.SuccessAsync("保存成功");
                await InvokeAsync(StateHasChanged); //如果进行了异步操作，需要强制刷新。在线程中更改了变量状态，需手动刷新。 
            });
        }
    }
}
