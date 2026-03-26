using Infrastructure.MemoryBus.Base;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;

namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Commands;

public class CodeTemplateBatchUpdateCommand : Command
{
    public IEnumerable<CodeTemplateVM> Vms { get; private set; }
    public CodeTemplateBatchUpdateCommand(IEnumerable<CodeTemplateVM> Vms)
    {
        Vms = Vms;
    }
}