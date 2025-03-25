using Modules.Core.Shared.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Modules.Tasks.Shared.Base_sys_tasks_log
{
    public class Base_sys_tasks_logVM : BaseVM
    {
        /*
         * 
         * �����������Ḳ�Ǳ��֮������ݡ�
         */
        //CodeGenerator start
        [DisplayName("����ID")]
        [Required(ErrorMessage = "����ID����Ϊ��")]
        public string Task_id { get; set; }
        [DisplayName("Ӧ�ó���ID")]
        public string App_id { get; set; }
        [DisplayName("��־����")]
        public string Log_type { get; set; }
        [DisplayName("ʱ��")]
        public DateTime? Log_time { get; set; }
        [DisplayName("��Ϣ")]
        public string Log_msg { get; set; }
        [DisplayName("����")]
        public string Log_text { get; set; }
        [DisplayName("ִ��ʱ��")]
        public string Milliseconds { get; set; }
        [DisplayName("�������")]
        public string Solution_id { get; set; }
        [DisplayName("�⻧ID")]
        [Required(ErrorMessage = "�⻧ID����Ϊ��")]
        public string Tenant_id { get; set; }
        //CodeGenerator end
    }
}
