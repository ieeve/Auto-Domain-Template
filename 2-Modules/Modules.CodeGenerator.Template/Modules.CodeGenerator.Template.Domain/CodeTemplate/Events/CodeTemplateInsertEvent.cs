using Infrastructure.MemoryBus.Base;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;

namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Events
{
    public class CodeTemplateUpdatedEvent : Event
    {
        public CodeTemplateVM Vm { get; set; }
        public CodeTemplateUpdatedEvent(CodeTemplateVM VM)
        {
            this.Vm = VM;
        }

    }
}
