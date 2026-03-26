using Infrastructure.MemoryBus.Base;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;

namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Events
{
    public class CodeTemplateBatchUpdatedEvent : Event
    {
        public IEnumerable<CodeTemplateVM> Vms { get; private set; }
        public CodeTemplateBatchUpdatedEvent(IEnumerable<CodeTemplateVM> Vms)
        {
            this.Vms = Vms;
        }

    }
}
