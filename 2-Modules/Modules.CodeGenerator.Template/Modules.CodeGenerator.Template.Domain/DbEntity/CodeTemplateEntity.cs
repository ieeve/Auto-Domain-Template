using SqlSugar;

namespace Modules.CodeGenerator.Template.Domain.DbEntity
{
    [SugarTable(TableName = "CodeTemplate", TableDescription = "模板表")]
    public class CodeTemplateEntity : Core.Domain.Base.Entity
    {

        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "用户名")]
        public string username { get; set; }

        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "密码")]
        public string password { get; set; }

        [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "状态")]
        public string state { get; set; }

        [SugarColumn(IsNullable = true, Length = 255, ColumnDescription = "说明")]
        public string notes { get; set; }
    }
}
