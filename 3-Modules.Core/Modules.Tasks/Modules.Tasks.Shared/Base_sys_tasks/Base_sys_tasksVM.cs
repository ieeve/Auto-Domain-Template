using Modules.Core.Shared.Base;
using Modules.Tasks.Shared.Constants;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Modules.Tasks.Shared.Base_sys_tasks
{
    public class Base_sys_tasksVM : BaseVM
    {
        [DisplayName("任务解决方案")]
        public string Task_solution_id { get; set; }
        [DisplayName("处理程序解决方案")]
        public string Process_solution_id { get; set; }
        [DisplayName("租户ID")]
        [Required(ErrorMessage = "租户ID不能为空")]
        public string Tenant_id { get; set; }
        [DisplayName("任务名称")]
        [Required(ErrorMessage = "任务名称不能为空")]
        public string Name { get; set; }
        [DisplayName("任务分组")]
        public string Jobgroup { get; set; }
        [DisplayName("任务描述")]
        public string Notes { get; set; }
        [DisplayName("运行日志")]
        [Required(ErrorMessage = "运行日志不能为空")]
        public TaskLog_Level Log_level { get; set; } = TaskLog_Level.记录错误;
        [DisplayName("累计执行次数")]
        [Required(ErrorMessage = "累计执行次数不能为空")]
        public int Run_times { get; set; }
        [DisplayName("是否启动")]
        [Required(ErrorMessage = "是否启动不能为空")]
        public bool Is_start { get; set; }
        [DisplayName("表达式是否Cron")]
        [Required(ErrorMessage = "表达式是否Cron不能为空")]
        public bool Is_cron { get; set; }
        [DisplayName("执行间隔时间(单位:秒)")]
        [Required(ErrorMessage = "执行间隔时间(单位:秒)不能为空")]
        public int Interval_second { get; set; }
        [DisplayName("运行时间表达式")]
        public string Cron { get; set; }
        [DisplayName("任务类型")]
        [Required(ErrorMessage = "任务类型不能为空")]
        public TaskType Task_type { get; set; }
        [DisplayName("GET/POST")]
        [Required(ErrorMessage = "GET/POST不能为空")]
        public RequestMethod Request_method { get; set; }
        [DisplayName("Post参数")]
        public string Request_param { get; set; }
        [DisplayName("API地址")]
        public string Api_url { get; set; }
        [DisplayName("处理程序API地址")]
        public string Process_api_url { get; set; }
        [DisplayName("任务所在类")]
        public string Class_name { get; set; }
        [DisplayName("执行传参")]
        public string Job_params { get; set; }
        [DisplayName("处理程序任务类型")]
        [Required(ErrorMessage = "处理程序任务类型不能为空")]
        public TaskType Process_task_type { get; set; } = TaskType.无;
        [DisplayName("处理程序GET/POST")]
        [Required(ErrorMessage = "处理程序GET/POST不能为空")]
        public RequestMethod Process_request_method { get; set; }
        [DisplayName("处理程序HTTP请求值")]
        public string Process_request_param { get; set; }
        [DisplayName("处理程序API地址")]
        public string Process_assembly_name { get; set; }
        [DisplayName("处理程序所在类")]
        public string Process_class_name { get; set; }
        [DisplayName("处理程序传入参数")]
        public string Process_job_params { get; set; }
        [DisplayName("任务sql")]
        public string Sql { get; set; }
        [DisplayName("Sql存储过程")]
        public string Sql_function { get; set; }

        [DisplayName("处理程序SQL语句")]
        public string Process_sql { get; set; }
        [DisplayName("处理程序存储过程")]
        public string Process_sql_function { get; set; }

        [DisplayName("最后一次正确执行时间")]
        public DateTime? Last_success_time { get; set; }
    }
}
