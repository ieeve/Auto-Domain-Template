using Modules.Core.Shared.Base;
using Modules.Tasks.Shared.Constants;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Modules.Tasks.Shared.Base_sys_tasks
{
    public class Base_sys_tasksVM : BaseVM
    {
        [DisplayName("����������")]
        public string Task_solution_id { get; set; }
        [DisplayName("�������������")]
        public string Process_solution_id { get; set; }
        [DisplayName("�⻧ID")]
        [Required(ErrorMessage = "�⻧ID����Ϊ��")]
        public string Tenant_id { get; set; }
        [DisplayName("��������")]
        [Required(ErrorMessage = "�������Ʋ���Ϊ��")]
        public string Name { get; set; }
        [DisplayName("�������")]
        public string Jobgroup { get; set; }
        [DisplayName("��������")]
        public string Notes { get; set; }
        [DisplayName("������־")]
        [Required(ErrorMessage = "������־����Ϊ��")]
        public TaskLog_Level Log_level { get; set; } = TaskLog_Level.��¼����;
        [DisplayName("�ۼ�ִ�д���")]
        [Required(ErrorMessage = "�ۼ�ִ�д�������Ϊ��")]
        public int Run_times { get; set; }
        [DisplayName("�Ƿ�����")]
        [Required(ErrorMessage = "�Ƿ���������Ϊ��")]
        public bool Is_start { get; set; }
        [DisplayName("���ʽ�Ƿ�Cron")]
        [Required(ErrorMessage = "���ʽ�Ƿ�Cron����Ϊ��")]
        public bool Is_cron { get; set; }
        [DisplayName("ִ�м��ʱ��(��λ:��)")]
        [Required(ErrorMessage = "ִ�м��ʱ��(��λ:��)����Ϊ��")]
        public int Interval_second { get; set; }
        [DisplayName("����ʱ����ʽ")]
        public string Cron { get; set; }
        [DisplayName("��������")]
        [Required(ErrorMessage = "�������Ͳ���Ϊ��")]
        public TaskType Task_type { get; set; }
        [DisplayName("GET/POST")]
        [Required(ErrorMessage = "GET/POST����Ϊ��")]
        public RequestMethod Request_method { get; set; }
        [DisplayName("Post����")]
        public string Request_param { get; set; }
        [DisplayName("API��ַ")]
        public string Api_url { get; set; }
        [DisplayName("�������API��ַ")]
        public string Process_api_url { get; set; }
        [DisplayName("����������")]
        public string Class_name { get; set; }
        [DisplayName("ִ�д���")]
        public string Job_params { get; set; }
        [DisplayName("���������������")]
        [Required(ErrorMessage = "��������������Ͳ���Ϊ��")]
        public TaskType Process_task_type { get; set; } = TaskType.��;
        [DisplayName("�������GET/POST")]
        [Required(ErrorMessage = "�������GET/POST����Ϊ��")]
        public RequestMethod Process_request_method { get; set; }
        [DisplayName("�������HTTP����ֵ")]
        public string Process_request_param { get; set; }
        [DisplayName("�������API��ַ")]
        public string Process_assembly_name { get; set; }
        [DisplayName("�������������")]
        public string Process_class_name { get; set; }
        [DisplayName("������������")]
        public string Process_job_params { get; set; }
        [DisplayName("����sql")]
        public string Sql { get; set; }
        [DisplayName("Sql�洢����")]
        public string Sql_function { get; set; }

        [DisplayName("�������SQL���")]
        public string Process_sql { get; set; }
        [DisplayName("�������洢����")]
        public string Process_sql_function { get; set; }

        [DisplayName("���һ����ȷִ��ʱ��")]
        public DateTime? Last_success_time { get; set; }
    }
}
