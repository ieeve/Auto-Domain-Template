using Force.DeepCloner;
using MapsterMapper;
using Modules.CodeGenerator.Template.Domain.CodeTemplate.Events;
using NetDevPack.SimpleMediator;

namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Handler
{
    public class CodeTemplateDeletedEventHandler : INotificationHandler<CodeTemplateDeletedEvent>, INotificationHandler<CodeTemplateBatchDeletedEvent>
    {
        private readonly ICodeTemplateRepository _Repository;
        private readonly IMapper _mapper;
        public CodeTemplateDeletedEventHandler(IMapper Mapper, ICodeTemplateRepository repository)
        {
            _mapper = Mapper;
            _Repository = repository;
        }

        public async Task Handle(CodeTemplateDeletedEvent notification, CancellationToken cancellationToken)
        {


        }

        public async Task Handle(CodeTemplateBatchDeletedEvent notification, CancellationToken cancellationToken)
        {

        }

    }
}
