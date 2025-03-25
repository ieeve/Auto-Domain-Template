using System.Text.RegularExpressions;

namespace Modules.Domain
{
    /// <summary>
    /// 此类用于实现Unicode字符与中文字符的相互转换
    /// </summary>
    public class UnicodeConverter
    {
        /// <summary>
        /// 将中文字符串转化为Unicode串
        /// </summary>
        public static String ToUnicode(String str)
        {
            String tmp = "";

            foreach (char C in str)
            {
                if (isChinese(C)) tmp += "\\u" + ToString(C, 16);   // 将中文字符转化为Unicode串
                else tmp += C;
            }
            return tmp;
        }

        /// <summary>
        /// 将Unicode串转化为中文字符串
        /// </summary>
        public static String ToChinese(String str)
        {
            String tmp = str;

            Regex regex = new Regex("\\\\u[0-9a-fA-F]{4}", RegexOptions.IgnoreCase);
            MatchCollection collection = regex.Matches(tmp);
            foreach (Match match in collection)
            {
                String hexStr = match.Value.Substring("\\u".Length); // 获取16进制串
                String C = ToChar(match.Value, 16).ToString();
                tmp = tmp.Replace(match.Value, C);
            }

            return tmp;
        }


        // 相关功能函数
        // -----------------------------------------------------------------

        // 判断字符C是否是中文字符
        public static bool isChinese(char C)
        {
            // 中文字符范围
            return 0x4e00 <= C && C <= 0x9fbb;
        }

        // 将数值num转化为radix进制表示的字符串, 2 <= radix <= 36
        public static String ToString(int num, int radix = 10)
        {
            String Str = "";
            while (num > 0)
            {
                int remainder = num % radix;    // 取余数
                num = num / radix;              // 取商

                Str = ToChar(remainder) + Str;  // 将各位余数，依次转化为对应进制字符
            }
            return Str;
        }

        // 将数值n转化为字符，0 <= n <= 35，依次转化为字符0-9a-z;
        // 最大可表示36进制数
        public static char ToChar(int n)
        {
            n = n % 36;

            if (n < 10) n += '0';
            else n += 'a' - 10;

            return (char)n;
        }


        // 将字符0-9a-z依次转化为数值0-35
        public static int ToInt(char C)
        {
            if (C > '9') return C - 'a' + 10;
            else return C - '0';
        }

        // 将字符串radix进制的串str转化为字符
        public static char ToChar(String str, int radix)
        {
            int n = 0;

            foreach (char C in str)
            {
                n = n * radix + ToInt(C);
            }

            return (char)n;
        }
    }
}
