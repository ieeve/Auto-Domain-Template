using System.ComponentModel;
using System.Reflection;

namespace Infrastructure.Common.Helpers
{
    public class CompareDataEntityHelper
    {
        /// <summary>
        /// 比较两个实体类差异
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newModel">新的model</param>
        /// <param name="oldModel">原来的model</param>
        /// <param name="IgnoreField">不对比某些字段</param>
        /// <returns>List<string> compstrlst 就是变动的描述，多个字段变动，则多条描述。</returns>
        public static List<string> CompareToString<T>(T newModel, T oldModel, List<string>? IgnoreField = null)
        {
            if (IgnoreField == null)
            {
                IgnoreField = new List<string>() { "Id", "Createuid", "Createtime", "Updateuid", "Updatetime", "Row_CSS_Class", "Row_no", "Parentid", "SqlsugarTreeChild" };
            }
            IgnoreField = IgnoreField.ConvertAll(x => x = x.ToLower()); //全部转为小写，便于后面的对比

            List<string> data = new List<string>();
            if (newModel == null || oldModel == null)
            {
                return new List<string>();
            }
            PropertyInfo[] newProperties = newModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] oldProperties = oldModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (newProperties.Length <= 0 || oldProperties.Length <= 0)
            {
                return new List<string>();
            }

            var oldFieldLst = new Dictionary<string, string>();
            foreach (PropertyInfo item in oldProperties)
            {
                string filedName = item.Name;//实体类字段名称
                string value = item.GetValue(oldModel, null)?.ToString() ?? "";//该字段的值
                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                {
                    oldFieldLst.Add(filedName, value);//在此可转换value的类型
                }
            }
            foreach (PropertyInfo item in newProperties)
            {
                string filedName = item.Name;//实体类字段名称

                if (IgnoreField.Contains(filedName.ToLower()))
                {
                    continue;
                }
                //显示[DisplayName] 名
                var displayName = string.Empty;
                var attrs = item.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    displayName = ((DisplayNameAttribute)attrs[0]).DisplayName;
                }
                else
                {
                    displayName = filedName;
                }

                string value = item.GetValue(newModel, null)?.ToString() ?? "";//该字段的值
                if (oldFieldLst.ContainsKey(filedName))
                {
                    string olddata = oldFieldLst[filedName];
                    if (olddata != value)
                    {
                        data.Add($"修改【{displayName}】:[{olddata}] 改为 [{value}] ");
                    }
                }
                else
                {
                    data.Add($"新增[{displayName}]");
                }

            }
            return data;
        }
    }

    /// <summary>
    /// 字段名标注
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class FieldNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public FieldNameAttribute(string name)
        {

        }
    }
}
