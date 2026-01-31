using Modules.Core.Domain.Interfaces;

namespace Modules.My.Domain.Base
{
    /// <summary>
    ///     仓储通用接口类
    /// </summary>
    /// <typeparam name="T">泛型实体类</typeparam>
    public interface IBaseRepository<T> : ICoreRepository<T>, IDisposable where T : class, new()
    {
    }
}