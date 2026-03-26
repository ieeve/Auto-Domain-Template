using Force.DeepCloner;
using MapsterMapper;
using Modules.CodeGenerator.Template.Domain.CodeTemplate.Events;
using NetDevPack.SimpleMediator;

namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Handler
{
    public class CodeTemplateInsertEventHandler : INotificationHandler<CodeTemplateInsertEvent>, INotificationHandler<CodeTemplateBatchInsertEvent>
    {
        private readonly ICodeTemplateRepository _Repository;
        private readonly IMapper _mapper;
        public CodeTemplateInsertEventHandler(IMapper Mapper, ICodeTemplateRepository repository)
        {
            _mapper = Mapper;
            _Repository = repository;
        }

        public async Task Handle(CodeTemplateInsertEvent notification, CancellationToken cancellationToken)
        {


        }

        public async Task Handle(CodeTemplateBatchInsertEvent notification, CancellationToken cancellationToken)
        {

        }

    }
}
