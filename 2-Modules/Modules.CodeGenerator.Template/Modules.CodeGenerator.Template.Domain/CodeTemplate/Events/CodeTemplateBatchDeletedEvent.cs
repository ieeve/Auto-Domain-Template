using Infrastructure.MemoryBus.Base;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;

namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Events
{
    public class CodeTemplateBatchDeletedEvent : Event
    {
        public IEnumerable<CodeTemplateVM> Vms { get; protected set; }
        public CodeTemplateBatchDeletedEvent(IEnumerable<CodeTemplateVM> Vms)
        {
            this.Vms = Vms;
        }

    }
}
