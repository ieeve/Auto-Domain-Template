using SqlSugar;

namespace Modules.My.Domain.DbEntity
{
    [SugarTable(TableName = "CodeTemplate")]
    public class CodeTemplateTreeEntity : Core.Domain.Base.TreeEntity<CodeTemplateTreeEntity>
    {

        [SugarColumn(IsNullable = true)]
        public string username { get; set; }

        [SugarColumn(IsNullable = true)]
        public string password { get; set; }

        [SugarColumn(IsNullable = true)]
        public string notes { get; set; }

    }
}
