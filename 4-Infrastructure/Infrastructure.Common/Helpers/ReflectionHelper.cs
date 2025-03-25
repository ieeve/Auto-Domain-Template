using System.Reflection;

namespace Infrastructure.Common.Helpers
{
    public static class ReflectionHelper
    {
        //通过对象属性的值
        public static object GetValue<TSource>(TSource source, string propertyName)
        {
            if (source == null) return string.Empty;
            //忽略大小写
            var propInfo = source.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
            if (propInfo == null) return string.Empty;
            var val = propInfo.GetValue(source, null);
            return val ?? string.Empty;
        }
        public static void SetProperty<TModel>(TModel instance, string propertyName, object value)
        {
            var info = GetPropertyInformation(instance.GetType(), propertyName);

            var setMethod = info.GetSetMethod();
            if (setMethod != null)
            {
                setMethod.Invoke(instance, new object[] { value });
            }
            else
            {
                info.SetValue(instance, value);
            }
        }
        public static PropertyInfo GetPropertyInformation(Type type, string propertyName)
        {
            if (type == null)
                return null;
            var runTimeProps = type.GetRuntimeProperties().ToList();
            if (runTimeProps.Any())
                return runTimeProps.FirstOrDefault(p => p.Name == propertyName);
            return null;
        }
    }
}
