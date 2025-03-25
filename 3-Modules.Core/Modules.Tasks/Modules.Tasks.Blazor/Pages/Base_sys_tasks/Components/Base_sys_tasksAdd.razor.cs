using Force.DeepCloner;
using Microsoft.AspNetCore.Components;
using Modules.Core.AppServices.DbMaintenance;
using Modules.Core.Shared.Base_solution;
using Modules.Tasks.AppServices.Base_sys_tasks;
using Modules.Tasks.Shared.Base_sys_tasks;
using Modules.Tasks.Shared.Constants;

namespace Modules.Tasks.Blazor.Pages.Base_sys_tasks.Components
{
    partial class Base_sys_tasksAdd
    {
        //注入服务
        [Inject] private IBase_sys_tasksPageService _service { get; set; }
        //[Inject] private IDbConfigService _DbconfigService { get; set; }

        [Inject] private IDbMaintenanceService _dbMaintenanceService { get; set; }
        //组件之间传值
        [Parameter] public Base_sys_tasksVM model { get; set; }
        [Parameter] public bool IsAdd { get; set; }
        [Parameter] public EventCallback<bool> OnValueCallback { get; set; }

        private bool btnLoading = false;
        private async Task SubmitForm()
        {
            //验证
            if (model.Task_type == TaskType.存储过程 || model.Task_type == TaskType.Sql语句)
            {
                if (string.IsNullOrWhiteSpace(model.Task_solution_id))
                {
                    _= _message.Error("任务的解决方案不能为空。");
                    return;
                }
            }

            if (model.Task_type == TaskType.WebApi)
            {
                if (string.IsNullOrWhiteSpace(model.Api_url) || !model.Api_url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    _ = _message.Error("Url地址必须是http或者https开头。");
                    return;
                }
            }

            if (model.Process_task_type == TaskType.存储过程 || model.Process_task_type == TaskType.Sql语句)
            {
                if (string.IsNullOrWhiteSpace(model.Process_solution_id))
                {
                    _= _message.Error("处理程序的解决方案不能为空。");
                    return;
                }
            }

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
        void TaskTypeChange(TaskType taskType)
        {
            if (taskType == TaskType.WebApi)
            {
                // model.Assembly_name = "Modules.Tasks.AppServices";
                model.Class_name = "Job_HttpApi_Quartz";
            }
            if (taskType == TaskType.程序集)
            {
                //model.Assembly_name = "Modules.Tasks.AppServices";
                // model.Class_name = "";
            }
        }
        // 解决方案（取得数据库ID），一个解决方案只能对应一个ID
        private List<string> TaskProcList = new List<string>(); //数据表存储过程列表
        private void TaskSolutionSelectChanged(Base_solutionVM Solution)
        {
            //this.model.Task_solution_id = Solution.Id;
            this.TaskProcList = _dbMaintenanceService.GetProcList(Solution.Dbid);
        }
        private List<string> ProcessProcList = new List<string>(); //数据表存储过程列表
        private void ProcessSolutionSelectChanged(Base_solutionVM Solution)
        {
            //this.model.Process_solution_id = Solution.Id;
            this.ProcessProcList = _dbMaintenanceService.GetProcList(Solution.Dbid);
        }



    }
}