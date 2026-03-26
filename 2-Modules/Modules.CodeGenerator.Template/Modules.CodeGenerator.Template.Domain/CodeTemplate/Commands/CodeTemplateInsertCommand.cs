using Infrastructure.MemoryBus.Base;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;

namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Commands;

public class CodeTemplateInsertCommand : Command
{
    public CodeTemplateVM Vm { get; private set; }
    public CodeTemplateInsertCommand(CodeTemplateVM Vm)
    {
        Vm = Vm;
    }
}
