using Infrastructure.MemoryBus;
using Infrastructure.MemoryBus.Base;
using MapsterMapper;
using Microsoft.AspNetCore.Components.Forms;
using Modules.CodeGenerator.Template.Domain.CodeTemplate.Commands;
using Modules.CodeGenerator.Template.Domain.CodeTemplate.Events;
using Modules.CodeGenerator.Template.Domain.DbEntity;
using Modules.Core.Shared;
using NetDevPack.SimpleMediator;


namespace Modules.CodeGenerator.Template.Domain.CodeTemplate.Handler
{
    public class CodeTemplateInsertCommandHandler : CommandHandler,
        IRequestHandler<CodeTemplateInsertCommand, Result>,
        IRequestHandler<CodeTemplateBatchInsertCommand, Result>
    {
        private readonly ICodeTemplateRepository _repository;
        private readonly IMapper _mapper;
        private IMediatorHandler _mediator;
        public CodeTemplateInsertCommandHandler(ICodeTemplateRepository Repository, IMapper mapper, IMediatorHandler mediator)
        {
            _repository = Repository;
            _mapper = mapper;
            this._mediator = mediator;
        }

        public async Task<Result> Handle(CodeTemplateInsertCommand cmd, CancellationToken cancellationToken)
        {
            //1. 验证
            if (!cmd.IsValid()) return Result.Warning(cmd.ErrorMessages());
            //2. 执行存储
            var entity = _mapper.Map<CodeTemplateEntity>(cmd.Vm);
            var ret = await _repository.InsertAsync(entity);
            if (ret <= 0) return Result.Error("更新失败");
            //3. 发送事件
            await _mediator.PublishEvent(new CodeTemplateInsertEvent(cmd.Vm));
            return Result.Success("添加成功", entity.id);
        }

        public async Task<Result> Handle(CodeTemplateBatchInsertCommand cmd, CancellationToken cancellationToken)
        {
            //1. 验证
            if (!cmd.IsValid()) return Result.Warning(cmd.ErrorMessages());

            var entity = _mapper.Map<List<CodeTemplateEntity>>(cmd.Vms);
            var ret = await _repository.InsertAsync(entity);
            if (ret <= 0) return Result.Error("更新失败");
            //3. 发送事件
            await _mediator.PublishEvent(new CodeTemplateBatchInsertEvent(cmd.Vms));
            return Result.Success("更新成功");
        }
        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}