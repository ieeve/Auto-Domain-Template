using Infrastructure.MemoryBus.Base;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;

namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Commands;

public class CodeTemplateBatchDeleteCommand : Command
{
    public IEnumerable<CodeTemplateVM> Vms { get; private set; }
    public CodeTemplateBatchDeleteCommand(IEnumerable<CodeTemplateVM> Vms)
    {
        Vms = Vms;
    }
}