using System.ComponentModel;
using System.Reflection;

namespace Infrastructure.Common.Helpers
{
    /// <summary>
    /// 枚举操作类
    /// </summary>
    public static class EnumHepler
    {

        /// <summary>
        /// 获取枚举集合
        /// </summary>
        /// <param name="enumType">枚举</param>
        /// <returns>键值</returns>
        public static Dictionary<int, string> GetEnumItems(Type enumType)
        {
            FieldInfo[] enumItems = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            Dictionary<int, string> names = new Dictionary<int, string>();

            foreach (FieldInfo enumItem in enumItems)
            {
                names.Add((int)Enum.Parse(enumType, enumItem.Name), enumItem.Name);
            }
            return names;
        }
        /// <summary>
        /// 获取枚举描述集合
        /// </summary>
        /// <param name="enumType">枚举</param>
        /// <returns>键值</returns>
        public static Dictionary<string, string> GetEnumDescriptionItems(Type enumType)
        {
            FieldInfo[] enumItems = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            Dictionary<string, string> names = new Dictionary<string, string>();

            foreach (FieldInfo enumItem in enumItems)
            {
                // 获取描述
                var attr = enumItem.GetCustomAttribute(typeof(DescriptionAttribute), true) as DescriptionAttribute;
                if (attr != null && !string.IsNullOrEmpty(attr.Description))
                {
                    names.Add(attr.Description, attr.Description);
                }
            }
            return names;
        }
        /// <summary>
        /// 获取枚举描述值
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns>值</returns>
        public static string GetEnumDescription(Enum enumValue)
        {
            string str = enumValue.ToString();
            var field = enumValue.GetType().GetField(str);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (objs.Length == 0) return str;
            DescriptionAttribute da = (DescriptionAttribute)objs[0];
            return da.Description;
        }
    }
}
