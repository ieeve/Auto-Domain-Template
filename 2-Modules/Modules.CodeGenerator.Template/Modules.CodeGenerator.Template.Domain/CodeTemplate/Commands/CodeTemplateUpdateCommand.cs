using Infrastructure.MemoryBus.Base;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;

namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Commands;

public class CodeTemplateUpdateCommand : Command
{
    public CodeTemplateVM Vm { get; private set; }
    public CodeTemplateUpdateCommand(CodeTemplateVM Vm)
    {
        Vm = Vm;
    }
}
