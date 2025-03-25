using SqlSugar;

namespace Modules.Template.Domain.DbEntity
{
    [SugarTable("CodeTemplate")]
    public class CodeTemplateEntity : Core.Domain.Base.Entity
    {
        [SugarColumn(IsNullable = true)]
        public string username { get; set; }

        [SugarColumn(IsNullable = true)]
        public string password { get; set; }

        [SugarColumn(IsNullable = true)]
        public string notes { get; set; }
    }
}
