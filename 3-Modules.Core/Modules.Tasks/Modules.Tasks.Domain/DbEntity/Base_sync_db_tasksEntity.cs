using Modules.Core.Domain.Base;
using SqlSugar;

namespace Modules.Tasks.Domain.DbEntity
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("base_sync_db_tasks")]
    public partial class Base_sync_db_tasksEntity : Entity
    {
        public Base_sync_db_tasksEntity()
        {
        }
        #region ��������

        [SugarColumn(Length = 255, ColumnDescription = "��������")]
        public string name { get; set; }

        [SugarColumn(Length = 1000, IsNullable = true, ColumnDescription = "��������")]
        public string notes { get; set; }

        [SugarColumn(DefaultValue = "0", ColumnDescription = "������־")]
        public int log_level { get; set; }

        [SugarColumn(DefaultValue = "0", ColumnDescription = "�ۼ�ִ�д���")]
        public int run_times { get; set; }

        [SugarColumn(DefaultValue = "0", ColumnDescription = "�Ƿ�����")]
        public bool is_start { get; set; }

        #endregion
        #region ִ��ʱ������

        [SugarColumn(DefaultValue = "0", ColumnDescription = "���ʽ�Ƿ�Cron")]
        public bool is_cron { get; set; }

        [SugarColumn(DefaultValue = "0", ColumnDescription = "ִ�м��ʱ��(��λ:��)")]
        public int interval_second { get; set; }

        [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "����ʱ����ʽ")]
        public string cron { get; set; }

        #endregion

        #region �ݿ�����

        //ȥ������ID��ʹ�� ���ݿ�ID+���� ȷ��Ψһֵ
        [SugarColumn(Length = 36, IsNullable = false, ColumnDescription = "Դ���ݿ�ID")]
        public string source_db_id { get; set; }

        [SugarColumn(Length = 36, IsNullable = false, ColumnDescription = "Դ����")]
        public string source_table_name { get; set; }

        [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "Դ����ʱ������")]
        public string source_createtime { get; set; }

        [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "Դ�޸�ʱ������")]
        public string source_updatetime { get; set; }

        //Ŀ�����ݿ�
        [SugarColumn(Length = 36, IsNullable = false, ColumnDescription = "Ŀ�����ݿ�ID")]
        public string target_db_id { get; set; }

        [SugarColumn(Length = 36, IsNullable = false, ColumnDescription = "Ŀ�����")]
        public string target_table_name { get; set; }
        #endregion

        [SugarColumn(IsNullable = true, ColumnDescription = "���һ�γɹ�ִ�е�ʱ��")]
        public DateTime? last_success_time { get; set; }

        [SugarColumn(Length = 36, IsNullable = false, ColumnDescription = "�⻧ID")]
        public string tenant_id { get; set; }
    }
}
