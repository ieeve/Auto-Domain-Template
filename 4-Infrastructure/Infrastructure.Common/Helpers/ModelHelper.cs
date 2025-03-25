using System.Reflection;

namespace Infrastructure.Common.Helpers
{
    /// <summary>
    /// 实体操作类
    /// </summary>
    public class ModelHelper
    {
        /// <summary>
        /// 实体数据复制
        /// </summary>
        /// <typeparam name="TSource">要复制的实体</typeparam>
        /// <typeparam name="TArget">要赋值的实体</typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyValue<TSource, TArget>(TSource source, TArget target)
        {
            Type ts = source.GetType();
            Type to = target.GetType();

            foreach (PropertyInfo info in to.GetProperties())
            {
                PropertyInfo tsInfo = ts.GetProperty(info.Name);
                if (tsInfo != null)
                {
                    var tsValue = tsInfo.GetValue(source, null);
                    info.SetValue(target, tsValue, null);
                }
            }
        }
    }
}
