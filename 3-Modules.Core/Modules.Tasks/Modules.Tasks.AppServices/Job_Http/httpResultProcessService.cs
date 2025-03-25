using Modules.Tasks.AppServices.Base_sys_tasks_log;
using Modules.Tasks.Shared.Base_sys_tasks;
using System.Web;

namespace Modules.Tasks.AppServices.Job_Http
{
    public class httpResultProcessService
    {
        private IBase_sys_tasks_logService _TaskLogService;
        public httpResultProcessService(IBase_sys_tasks_logService TaskLogService)
        {
            _TaskLogService = TaskLogService;
        }

        internal async Task<string> processResult(Base_sys_tasksVM model, string msg)
        {
            if (string.IsNullOrEmpty(msg)) return "http请求无返回值";
            if (string.IsNullOrEmpty(model.Api_url)) return "API地址不能为空";

            if (model.Api_url.Contains("域名"))
            {
                //从url取得串号
                Uri myUri = new Uri(model.Api_url);
                string serialnumber = HttpUtility.ParseQueryString(myUri.Query).Get("typenum");
                if (await parseSoilData(serialnumber, msg))
                {
                    return $"处理数据成功";
                }
                else { return $"处理数据失败"; }
            }
            return $"http任务{model.Id}执行成功";
        }

        private async Task<bool> parseSoilData(string serialnumber, string httpMsg)
        {

            return false;
        }
    }
}

