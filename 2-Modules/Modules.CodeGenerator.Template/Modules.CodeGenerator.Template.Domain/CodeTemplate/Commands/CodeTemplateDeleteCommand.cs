using Infrastructure.MemoryBus.Base;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;

namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Commands;

public class CodeTemplateDeleteCommand : Command
{
    public CodeTemplateVM Vm { get; private set; }
    public CodeTemplateDeleteCommand(CodeTemplateVM Vm)
    {
        Vm = Vm;
    }
}
