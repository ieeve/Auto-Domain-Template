using Infrastructure.MemoryBus.Base;
using Modules.CodeGenerator.Template.Shared.CodeTemplate;

namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Events
{
    public class CodeTemplateBatchInsertEvent : Event
    {
        public IEnumerable<CodeTemplateVM> Vms { get; set; }
        public CodeTemplateBatchInsertEvent(IEnumerable<CodeTemplateVM> Vms)
        {
            this.Vms = Vms;
        }

    }
}
