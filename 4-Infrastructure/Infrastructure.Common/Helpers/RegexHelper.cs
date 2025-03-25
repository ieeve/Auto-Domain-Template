using System.Text.RegularExpressions;

namespace Infrastructure.Common.Helpers
{
    /// <summary>
    /// 正则操作类
    /// </summary>
    public class RegexHelper
    {
        /// <summary>
        /// 正则表达式验证（数字+字母）或者（数字+特殊字符）或者（字母+特殊字符），不能是纯数字、纯字母、纯特殊字符，即只要符合这3个组合其中之一都为true
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public static bool CheckNumCharSpecial(string value)
        {
            //[0-9a-zA-Z\w~!@#$%\^&*?]{8,}
            Regex reg1 = new Regex(@"^(?!^(\d+|[a-zA-Z]+|[~!@+=#$%^&*?-]+)$)^[\w~!@+=#$%\^&*?-]+$");
            return reg1.IsMatch(value.Trim());
        }

        /// <summary>
        /// 手机格式
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public static bool CheckPhone(string value)
        {
            //[0-9a-zA-Z\w~!@#$%\^&*?]{8,}
            Regex reg1 = new Regex(@"^1[0-9][0-9]\d{8}$");
            return reg1.IsMatch(value.Trim());
        }
        /// <summary>
        /// 长度
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static bool CheckLength(string value, int length)
        {
            return !(value.Length > length);
        }
        /// <summary>
        /// 真实姓名
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public static bool CheckSpecialCharacter(string value)
        {
            Regex reg1 = new Regex(@"^[A-Za-z0-9\u4e00-\u9fa5]+$");
            return reg1.IsMatch(value.Trim());
        }
        /// <summary>
        /// 身份证
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public static bool CheckIDcard(string value)
        {
            if (long.TryParse(value.Remove(17), out var n) == false || n < Math.Pow(10, 16) || long.TryParse(value.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证  
            }
            var address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(value.Remove(2), StringComparison.Ordinal) == -1)
            {
                return false;//省份验证  
            }
            var birth = value.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            if (DateTime.TryParse(birth, out _) == false)
            {
                return false;//生日验证  
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] ai = value.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(wi[i]) * int.Parse(ai[i].ToString());
            }

            Math.DivRem(sum, 11, out var y);
            if (arrVarifyCode[y] != value.Substring(17, 1).ToLower())
            {
                return false;//校验码验证  
            }
            return true;
        }
    }
}
