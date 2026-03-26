using Infrastructure.MemoryBus.Base;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;

namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Events
{
    public class CodeTemplateInsertEvent : Event
    {
        public CodeTemplateVM Vm { get; set; }
        public CodeTemplateInsertEvent(CodeTemplateVM VM)
        {
            this.Vm = VM;
        }

    }
}
