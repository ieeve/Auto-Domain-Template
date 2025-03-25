using Infrastructure.Common.Helpers;
using Modules.Tasks.AppServices.Base_sys_tasks;
using Modules.Tasks.AppServices.Base_sys_tasks_log;
using Modules.Tasks.Shared.Base_sys_tasks;
using Modules.Tasks.Shared.Constants;
using Quartz;

namespace Modules.Tasks.AppServices.Job_Http
{
    public class Job_HttpApi_Task : JobBase, IJob
    {
        httpResultProcessService _httpResultProcess;
        public Job_HttpApi_Task(IBase_sys_tasksService tasksQzServices, httpResultProcessService httpResultProcess, IBase_sys_tasks_logService qzRunLogService) : base(qzRunLogService, tasksQzServices)
        {
            _httpResultProcess = httpResultProcess;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            DateTime dateTime = DateTime.Now;
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            //var model = await _tasksQzServices.GetById(jobId);
            string TaskVM_Json = Convert.ToString(context.JobDetail.JobDataMap["TaskVM_Json"]);
            var TaskVm = System.Text.Json.JsonSerializer.Deserialize<Base_sys_tasksVM>(TaskVM_Json);

            var executeLog = await ExecuteJob(context, async () => await Run(TaskVm));
        }
        public async Task<string> Run(Base_sys_tasksVM model)
        {
            var msg = "";

            if (model.Task_type == TaskType.WebApi)
            {
                if (model.Request_method == RequestMethod.Get) //model.MethodType?.ToUpper() == "GET"
                {
                    var rep = await HttpHelper.GetAsyncWithHttpClient(model.Api_url);
                    msg = rep;
                }
                else
                {
                    var postData = model.Request_param; //JsonHelper.ToJson(model.RequestValue);
                    var rep = await HttpHelper.PostRequestWithHttpClient(model.Api_url, postData);
                    msg = rep;
                }
                msg = await _httpResultProcess.processResult(model, msg);
            }
            return msg;
        }

    }
}
