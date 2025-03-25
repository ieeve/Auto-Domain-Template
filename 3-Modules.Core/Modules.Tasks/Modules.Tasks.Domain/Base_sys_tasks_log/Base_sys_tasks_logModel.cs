using Modules.Core.Domain.Base;

namespace Modules.Tasks.Domain.Base_sys_tasks_log
{
    public class Base_sys_tasks_logModel : BaseValidation
    {
        /*
         * 
         * 代码生成器会覆盖标记之间的内容。
         */
        //CodeGenerator start
        public string Id { get; private set; }
        public string Task_id { get; private set; } //任务ID
        public string App_id { get; private set; } //应用程序ID
        public string Log_type { get; private set; } //日志类型
        public DateTime? Log_time { get; private set; } //时间
        public string Log_text { get; private set; } //内容
        public string Milliseconds { get; private set; } //执行时间

        public Base_sys_tasks_logModel(string Task_id, string App_id, string Log_type, DateTime? Log_time, string Log_text, string Milliseconds)
        {
            this.Task_id = Task_id;
            this.App_id = App_id;
            this.Log_type = Log_type;
            this.Log_time = Log_time;
            this.Log_text = Log_text;
            this.Milliseconds = Milliseconds;
            //CodeGenerator end
        }
        public Base_sys_tasks_logModel GetDefault()
        {

            return this;
        }
    }
}
