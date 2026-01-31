using Modules.Core.Security.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Modules.CodeGenerator.Template.Shared.CodeTemplate
{
    public class CodeTemplateTreeVM : BaseTreeVM<CodeTemplateTreeVM>
    {
        //CodeGenerator start
        [Required(ErrorMessage = "姓名必须填写")]
        [DisplayName("姓名")]
        public string Username { get; set; }
        [DisplayName("密码")]
        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }
        [DisplayName("说明")]
        public string Notes { get; set; }
        //CodeGenerator end
    }
}
