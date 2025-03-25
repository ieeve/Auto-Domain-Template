using Modules.Core.Shared.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Modules.Tasks.Shared.Base_sys_tasks_log
{
    public class Base_sys_tasks_logVM : BaseVM
    {
        /*
         * 
         * 代码生成器会覆盖标记之间的内容。
         */
        //CodeGenerator start
        [DisplayName("任务ID")]
        [Required(ErrorMessage = "任务ID不能为空")]
        public string Task_id { get; set; }
        [DisplayName("应用程序ID")]
        public string App_id { get; set; }
        [DisplayName("日志类型")]
        public string Log_type { get; set; }
        [DisplayName("时间")]
        public DateTime? Log_time { get; set; }
        [DisplayName("消息")]
        public string Log_msg { get; set; }
        [DisplayName("内容")]
        public string Log_text { get; set; }
        [DisplayName("执行时间")]
        public string Milliseconds { get; set; }
        [DisplayName("解决方案")]
        public string Solution_id { get; set; }
        [DisplayName("租户ID")]
        [Required(ErrorMessage = "租户ID不能为空")]
        public string Tenant_id { get; set; }
        //CodeGenerator end
    }
}
