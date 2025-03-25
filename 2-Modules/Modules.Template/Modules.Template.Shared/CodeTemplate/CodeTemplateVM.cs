using Modules.Core.Shared.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Modules.Template.Shared.CodeTemplate
{
    public class CodeTemplateVM : BaseVM
    {
        /*
         * 
         * 代码生成器会覆盖标记之间的内容。
         */
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
