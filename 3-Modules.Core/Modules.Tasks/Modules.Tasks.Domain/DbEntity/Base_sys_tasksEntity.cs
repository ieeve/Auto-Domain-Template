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
        #region 基本设置
        [SugarColumn(IsPrimaryKey = true, Length = 36, ColumnDescription = "ID")]
        public string ID { get; set; }

        [SugarColumn(Length = 255, ColumnDescription = "任务名称")]
        public string NAME { get; set; }


        [SugarColumn(DefaultValue = "0", ColumnDescription = "任务类型")]
        public int TASK_TYPE { get; set; }//任务类型{http,程序集，存储过程，sql语句}

        [SugarColumn(DefaultValue = "0", ColumnDescription = "累计执行次数")]
        public int RUN_TIMES { get; set; }
        [SugarColumn(DefaultValue = "0", ColumnDescription = "是否启动")]
        public bool IS_START { get; set; }

        #endregion

        [SugarColumn(IsNullable = true, ColumnDescription = "最后一次成功执行的时间")]
        public DateTime? LAST_SUCCESS_TIME { get; set; }

        #region 执行时间设置

        [SugarColumn(DefaultValue = "0", ColumnDescription = "表达式是否Cron")]
        public bool IS_CRON { get; set; }
        [SugarColumn(DefaultValue = "0", ColumnDescription = "执行间隔时间(单位:秒)")]
        public int INTERVAL_SECOND { get; set; }
        [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "运行时间表达式")]
        public string CRON { get; set; }

        #endregion

        [SugarColumn(DefaultValue = "0", ColumnDescription = "运行日志")]
        public int LOG_LEVEL { get; set; }

        #region 执行任务设置
        [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "任务分组")]
        public string JOBGROUP { get; set; }

        [SugarColumn(Length = 1000, IsNullable = true, ColumnDescription = "任务描述")]
        public string NOTES { get; set; }

        [SugarColumn(Length = 36, IsNullable = true, ColumnDescription = "解决方案")]
        public string TASK_SOLUTION_ID { get; set; }

        [SugarColumn(DefaultValue = "0", ColumnDescription = "GET/POST")]
        public int REQUEST_METHOD { get; set; }

        [SugarColumn(Length = 1000, IsNullable = true, ColumnDescription = "Post参数")]
        public string REQUEST_PARAM { get; set; } //Post参数

        [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "API地址")]
        public string API_URL { get; set; }

        //[SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "程序集名称")]
        //public string ASSEMBLY_NAME { get; set; } //不在使用，在软件里设置一定在 Modules.Tasks项目名

        [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "任务所在类")]
        public string CLASS_NAME { get; set; }

        [SugarColumn(Length = 1000, IsNullable = true, ColumnDescription = "执行传参")]
        public string JOB_PARAMS { get; set; }
        [SugarColumn(Length = 2000, IsNullable = true, ColumnDescription = "任务sql")]
        public string SQL { get; set; }
        [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "Sql存储过程")]
        public string SQL_FUNCTION { get; set; }

        #endregion
        #region 处理结果设置
        [SugarColumn(Length = 36, IsNullable = true, ColumnDescription = "处理程序解决方案")]
        public string PROCESS_SOLUTION_ID { get; set; }
        [SugarColumn(DefaultValue = "0", ColumnDescription = "处理程序任务类型")]
        public int PROCESS_TASK_TYPE { get; set; }//任务类型{http,程序集，存储过程，sql语句}

        [SugarColumn(DefaultValue = "0", ColumnDescription = "处理程序GET/POST")]
        public int PROCESS_REQUEST_METHOD { get; set; }

        [SugarColumn(Length = 1000, IsNullable = true, ColumnDescription = "处理程序HTTP请求值")]
        public string PROCESS_REQUEST_PARAM { get; set; } //HTTP请求参数

        [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "处理程序API地址")]
        public string PROCESS_API_URL { get; set; }

        [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "处理程序集名称")]
        //public string PROCESS_ASSEMBLY_NAME { get; set; }

        //[SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "处理程序所在类")]
        public string PROCESS_CLASS_NAME { get; set; }

        [SugarColumn(Length = 1000, IsNullable = true, ColumnDescription = "处理程序传入参数")]
        public string PROCESS_JOB_PARAMS { get; set; }
        [SugarColumn(Length = 2000, IsNullable = true, ColumnDescription = "处理程序sql")]
        public string PROCESS_SQL { get; set; }
        [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "处理程序存储过程")]
        public string PROCESS_SQL_FUNCTION { get; set; }

        #endregion

        [SugarColumn(Length = 36, IsNullable = false, ColumnDescription = "租户ID")]
        public string TENANT_ID { get; set; }

        [SugarColumn(Length = 36, ColumnDescription = "建立用户")]
        public string CREATEUID { get; set; }


        [SugarColumn(Length = 36, IsNullable = true, ColumnDescription = "更新用户")]
        public string UPDATEUID { get; set; }


        [SugarColumn(ColumnDescription = "建立时间")]
        public DateTime CREATETIME { get; set; }


        [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
        public DateTime? UPDATETIME { get; set; }

    }
}
