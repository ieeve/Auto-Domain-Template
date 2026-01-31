using Modules.Core.Domain.Base;

namespace Modules.My.Domain.CodeTemplate
{
    public class CodeTemplateModel : BaseValidation
    {
        /*
         * 
         * 代码生成器会覆盖标记之间的内容。
         */
        //CodeGenerator start
        public long ID { get; private set; }
        public string username { get; private set; }
        public string password { get; private set; }
        public string notes { get; private set; }
        public DateTime createtime { get; private set; }
        public CodeTemplateModel(long ID, string Username, string Password, string Notes, DateTime CreateTime)
        {
            this.ID = ID;
            this.username = Username;
            this.password = Password;
            this.notes = Notes;
            this.createtime = CreateTime;
            //CodeGenerator end
        }
        public CodeTemplateModel GetDefault()
        {

            return this;
        }
    }
}
