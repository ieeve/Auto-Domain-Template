using SqlSugar;

namespace Modules.Tasks.Domain.DbEntity
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("BASE_SYS_TASKS")]
    public partial class Base_sys_tasksEntity
    {
        public Base_sys_tasksEntity()
        {
        }
        #region ��������
        [SugarColumn(IsPrimaryKey = true, Length = 36, ColumnDescription = "ID")]
        public string ID { get; set; }

        [SugarColumn(Length = 255, ColumnDescription = "��������")]
        public string NAME { get; set; }


        [SugarColumn(DefaultValue = "0", ColumnDescription = "��������")]
        public int TASK_TYPE { get; set; }//��������{http,���򼯣��洢���̣�sql���}

        [SugarColumn(DefaultValue = "0", ColumnDescription = "�ۼ�ִ�д���")]
        public int RUN_TIMES { get; set; }
        [SugarColumn(DefaultValue = "0", ColumnDescription = "�Ƿ�����")]
        public bool IS_START { get; set; }

        #endregion

        [SugarColumn(IsNullable = true, ColumnDescription = "���һ�γɹ�ִ�е�ʱ��")]
        public DateTime? LAST_SUCCESS_TIME { get; set; }

        #region ִ��ʱ������

        [SugarColumn(DefaultValue = "0", ColumnDescription = "���ʽ�Ƿ�Cron")]
        public bool IS_CRON { get; set; }
        [SugarColumn(DefaultValue = "0", ColumnDescription = "ִ�м��ʱ��(��λ:��)")]
        public int INTERVAL_SECOND { get; set; }
        [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "����ʱ����ʽ")]
        public string CRON { get; set; }

        #endregion

        [SugarColumn(DefaultValue = "0", ColumnDescription = "������־")]
        public int LOG_LEVEL { get; set; }

        #region ִ����������
        [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "�������")]
        public string JOBGROUP { get; set; }

        [SugarColumn(Length = 1000, IsNullable = true, ColumnDescription = "��������")]
        public string NOTES { get; set; }

        [SugarColumn(Length = 36, IsNullable = true, ColumnDescription = "�������")]
        public string TASK_SOLUTION_ID { get; set; }

        [SugarColumn(DefaultValue = "0", ColumnDescription = "GET/POST")]
        public int REQUEST_METHOD { get; set; }

        [SugarColumn(Length = 1000, IsNullable = true, ColumnDescription = "Post����")]
        public string REQUEST_PARAM { get; set; } //Post����

        [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "API��ַ")]
        public string API_URL { get; set; }

        //[SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "��������")]
        //public string ASSEMBLY_NAME { get; set; } //����ʹ�ã������������һ���� Modules.Tasks��Ŀ��

        [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "����������")]
        public string CLASS_NAME { get; set; }

        [SugarColumn(Length = 1000, IsNullable = true, ColumnDescription = "ִ�д���")]
        public string JOB_PARAMS { get; set; }
        [SugarColumn(Length = 2000, IsNullable = true, ColumnDescription = "����sql")]
        public string SQL { get; set; }
        [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "Sql�洢����")]
        public string SQL_FUNCTION { get; set; }

        #endregion
        #region ����������
        [SugarColumn(Length = 36, IsNullable = true, ColumnDescription = "�������������")]
        public string PROCESS_SOLUTION_ID { get; set; }
        [SugarColumn(DefaultValue = "0", ColumnDescription = "���������������")]
        public int PROCESS_TASK_TYPE { get; set; }//��������{http,���򼯣��洢���̣�sql���}

        [SugarColumn(DefaultValue = "0", ColumnDescription = "�������GET/POST")]
        public int PROCESS_REQUEST_METHOD { get; set; }

        [SugarColumn(Length = 1000, IsNullable = true, ColumnDescription = "�������HTTP����ֵ")]
        public string PROCESS_REQUEST_PARAM { get; set; } //HTTP�������

        [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "�������API��ַ")]
        public string PROCESS_API_URL { get; set; }

        [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "�����������")]
        //public string PROCESS_ASSEMBLY_NAME { get; set; }

        //[SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "�������������")]
        public string PROCESS_CLASS_NAME { get; set; }

        [SugarColumn(Length = 1000, IsNullable = true, ColumnDescription = "������������")]
        public string PROCESS_JOB_PARAMS { get; set; }
        [SugarColumn(Length = 2000, IsNullable = true, ColumnDescription = "�������sql")]
        public string PROCESS_SQL { get; set; }
        [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "�������洢����")]
        public string PROCESS_SQL_FUNCTION { get; set; }

        #endregion

        [SugarColumn(Length = 36, IsNullable = false, ColumnDescription = "�⻧ID")]
        public string TENANT_ID { get; set; }

        [SugarColumn(Length = 36, ColumnDescription = "�����û�")]
        public string CREATEUID { get; set; }


        [SugarColumn(Length = 36, IsNullable = true, ColumnDescription = "�����û�")]
        public string UPDATEUID { get; set; }


        [SugarColumn(ColumnDescription = "����ʱ��")]
        public DateTime CREATETIME { get; set; }


        [SugarColumn(IsNullable = true, ColumnDescription = "����ʱ��")]
        public DateTime? UPDATETIME { get; set; }

    }
}
