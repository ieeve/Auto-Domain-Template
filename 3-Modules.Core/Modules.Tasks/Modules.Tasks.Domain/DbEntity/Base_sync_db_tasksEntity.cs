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
        #region 基本设置

        [SugarColumn(Length = 255, ColumnDescription = "任务名称")]
        public string name { get; set; }

        [SugarColumn(Length = 1000, IsNullable = true, ColumnDescription = "任务描述")]
        public string notes { get; set; }

        [SugarColumn(DefaultValue = "0", ColumnDescription = "运行日志")]
        public int log_level { get; set; }

        [SugarColumn(DefaultValue = "0", ColumnDescription = "累计执行次数")]
        public int run_times { get; set; }

        [SugarColumn(DefaultValue = "0", ColumnDescription = "是否启动")]
        public bool is_start { get; set; }

        #endregion
        #region 执行时间设置

        [SugarColumn(DefaultValue = "0", ColumnDescription = "表达式是否Cron")]
        public bool is_cron { get; set; }

        [SugarColumn(DefaultValue = "0", ColumnDescription = "执行间隔时间(单位:秒)")]
        public int interval_second { get; set; }

        [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "运行时间表达式")]
        public string cron { get; set; }

        #endregion

        #region 据库设置

        //去掉对象ID，使用 数据库ID+表名 确定唯一值
        [SugarColumn(Length = 36, IsNullable = false, ColumnDescription = "源数据库ID")]
        public string source_db_id { get; set; }

        [SugarColumn(Length = 36, IsNullable = false, ColumnDescription = "源表名")]
        public string source_table_name { get; set; }

        [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "源创建时间列名")]
        public string source_createtime { get; set; }

        [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "源修改时间列名")]
        public string source_updatetime { get; set; }

        //目标数据库
        [SugarColumn(Length = 36, IsNullable = false, ColumnDescription = "目标数据库ID")]
        public string target_db_id { get; set; }

        [SugarColumn(Length = 36, IsNullable = false, ColumnDescription = "目标表名")]
        public string target_table_name { get; set; }
        #endregion

        [SugarColumn(IsNullable = true, ColumnDescription = "最后一次成功执行的时间")]
        public DateTime? last_success_time { get; set; }

        [SugarColumn(Length = 36, IsNullable = false, ColumnDescription = "租户ID")]
        public string tenant_id { get; set; }
    }
}
