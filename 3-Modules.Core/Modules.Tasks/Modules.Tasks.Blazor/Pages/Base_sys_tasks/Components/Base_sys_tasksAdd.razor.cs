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
        //ע�����
        [Inject] private IBase_sys_tasksPageService _service { get; set; }
        //[Inject] private IDbConfigService _DbconfigService { get; set; }

        [Inject] private IDbMaintenanceService _dbMaintenanceService { get; set; }
        //���֮�䴫ֵ
        [Parameter] public Base_sys_tasksVM model { get; set; }
        [Parameter] public bool IsAdd { get; set; }
        [Parameter] public EventCallback<bool> OnValueCallback { get; set; }

        private bool btnLoading = false;
        private async Task SubmitForm()
        {
            //��֤
            if (model.Task_type == TaskType.�洢���� || model.Task_type == TaskType.Sql���)
            {
                if (string.IsNullOrWhiteSpace(model.Task_solution_id))
                {
                    _= _message.Error("����Ľ����������Ϊ�ա�");
                    return;
                }
            }

            if (model.Task_type == TaskType.WebApi)
            {
                if (string.IsNullOrWhiteSpace(model.Api_url) || !model.Api_url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    _ = _message.Error("Url��ַ������http����https��ͷ��");
                    return;
                }
            }

            if (model.Process_task_type == TaskType.�洢���� || model.Process_task_type == TaskType.Sql���)
            {
                if (string.IsNullOrWhiteSpace(model.Process_solution_id))
                {
                    _= _message.Error("�������Ľ����������Ϊ�ա�");
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
        void TaskTypeChange(TaskType taskType)
        {
            if (taskType == TaskType.WebApi)
            {
                // model.Assembly_name = "Modules.Tasks.AppServices";
                model.Class_name = "Job_HttpApi_Quartz";
            }
            if (taskType == TaskType.����)
            {
                //model.Assembly_name = "Modules.Tasks.AppServices";
                // model.Class_name = "";
            }
        }
        // ���������ȡ�����ݿ�ID����һ���������ֻ�ܶ�Ӧһ��ID
        private List<string> TaskProcList = new List<string>(); //���ݱ�洢�����б�
        private void TaskSolutionSelectChanged(Base_solutionVM Solution)
        {
            //this.model.Task_solution_id = Solution.Id;
            this.TaskProcList = _dbMaintenanceService.GetProcList(Solution.Dbid);
        }
        private List<string> ProcessProcList = new List<string>(); //���ݱ�洢�����б�
        private void ProcessSolutionSelectChanged(Base_solutionVM Solution)
        {
            //this.model.Process_solution_id = Solution.Id;
            this.ProcessProcList = _dbMaintenanceService.GetProcList(Solution.Dbid);
        }



    }
}