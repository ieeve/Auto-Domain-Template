using Infrastructure.MemoryBus.Base;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;

namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Commands;

public class CodeTemplateBatchInsertCommand : Command
{
    public IEnumerable<CodeTemplateVM> Vms { get; private set; }
    public CodeTemplateBatchInsertCommand(IEnumerable<CodeTemplateVM> Vms)
    {
        Vms = Vms;
    }
}