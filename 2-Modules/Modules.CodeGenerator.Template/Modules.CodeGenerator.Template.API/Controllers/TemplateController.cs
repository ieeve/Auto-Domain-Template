using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.CodeGenerator.Template.AppServices.CodeTemplate;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;
using System.Threading.Tasks;

namespace Modules.CodeGenerator.Template.API.Controllers
{
    [ApiController]
    [Tags("Template")]
    [Route("API/Template/[controller]/[action]")] //务必增加API/前缀
    [Authorize] //使用身份验证
    public class TemplateController : ControllerBase
    {
        private readonly ILogger<TemplateController> _logger;
        private readonly ICodeTemplateService _codeTemplateService;
        public TemplateController(ILogger<TemplateController> logger, ICodeTemplateService templateService)
        {
            _logger = logger;
            _codeTemplateService = templateService;
        }

        [HttpGet] //[HttpGet(Name = "GetById")] 
        [EndpointSummary("取得数据")]
        //[EndpointDescription("根据ID取得一条数据")]
        public async Task<CodeTemplateVM> GetById(string id)
        {
            return await _codeTemplateService.QueryVmByIdAsync(id);
        }

        [HttpPost]
        [EndpointSummary("更新数据")]
        //[EndpointDescription("前端Post更新数据")]
        public async Task<IActionResult> Update([FromBody] CodeTemplateVM data)
        {
            //取得当前登录用户名
            var userName = HttpContext.User.Identity?.Name;
            // await _codeTemplateService.UpdateAsync(data);
            return NoContent();
        }
    }
}
