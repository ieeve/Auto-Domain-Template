using Infrastructure.MemoryBus.Base;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;

namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Events
{
    public class CodeTemplateDeletedEvent : Event
    {
        public CodeTemplateVM Vm { get; set; }
        public CodeTemplateDeletedEvent(CodeTemplateVM VM)
        {
            this.Vm = VM;
        }

    }
}
