using MapsterMapper;
using Modules.CodeGenerator.Template.Domain.CodeTemplate.Events;
using NetDevPack.SimpleMediator;

namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Handler
{
    public class CodeTemplateUpdatedEventHandler : INotificationHandler<CodeTemplateUpdatedEvent>, INotificationHandler<CodeTemplateBatchUpdatedEvent>
    {
        private readonly ICodeTemplateRepository _Repository;
        private readonly IMapper _mapper;
        public CodeTemplateUpdatedEventHandler(IMapper Mapper, ICodeTemplateRepository repository)
        {
            _mapper = Mapper;
            _Repository = repository;
        }

        public async Task Handle(CodeTemplateUpdatedEvent notification, CancellationToken cancellationToken)
        {


        }

        public async Task Handle(CodeTemplateBatchUpdatedEvent notification, CancellationToken cancellationToken)
        {

        }

    }
}
