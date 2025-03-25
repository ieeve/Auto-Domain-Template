using SqlSugar;

namespace Modules.Tasks.Domain.DbEntity
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("BASE_SYS_TASKS_LOG")]
    public partial class Base_sys_tasks_logEntity
    {
        public Base_sys_tasks_logEntity()
        {


        }

        [SugarColumn(IsPrimaryKey = true, Length = 36)]
        public string ID { get; set; }

        [SugarColumn(Length = 36, ColumnDescription = "����ID")]
        public string TASK_ID { get; set; }

        [SugarColumn(Length = 36, IsNullable = true, ColumnDescription = "��־����")]
        public string LOG_TYPE { get; set; }

        [SugarColumn(IsNullable = true, ColumnDescription = "ʱ��")]
        public DateTime? LOG_TIME { get; set; }

        [SugarColumn(Length = 1000, IsNullable = true, ColumnDescription = "��Ϣ")]
        public string LOG_MSG { get; set; }

        [SugarColumn(Length = 2000, IsNullable = true, ColumnDescription = "����")]
        public string LOG_TEXT { get; set; }

        [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "ִ��ʱ��")]
        public string MILLISECONDS { get; set; }

        [SugarColumn(Length = 36, IsNullable = true, ColumnDescription = "�������")]
        public string SOLUTION_ID { get; set; }

        [SugarColumn(Length = 36, IsNullable = false, ColumnDescription = "�⻧ID")]
        public string TENANT_ID { get; set; }

        [SugarColumn(Length = 36, IsNullable = true, ColumnDescription = "Ӧ�ó���ID")]
        public string APP_ID { get; set; }
    }
}
