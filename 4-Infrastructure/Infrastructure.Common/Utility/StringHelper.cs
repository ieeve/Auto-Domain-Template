using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Common.Utility
{
    public static class StringHelper
    {
        /// <summary>
        /// 从字符串前面删除指定字符个数
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="len">个数</param>
        /// <returns>返回删除后的字符串</returns>
        public static string RemoveLeft(string s, int len)
        {
            return s.PadLeft(len).Remove(0, len);
        }


        /// <summary>
        /// 从字符串后面删除指定字符个数
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="len">个数</param>
        /// <returns>返回删除后的字符串</returns>
        public static string RemoveRight(string s, int len)
        {
            s = s.PadRight(len);
            return s.Remove(s.Length - len, len);
        }
        /// <summary>
        /// 去除原字符串结尾处的所有替换字符串
        /// 即使是 TirmEnd("abcd".ToCharArray()) ，其作用也是删除字符（删除尾部出现的a或b或c或d字符，删除的过程直到碰到一个既不是a也不是b也不是c也不是d的字符才结束），而不是删除字符串“abcd”。
        /// 如：原字符串"sdlfjdcdcd",替换字符串"cd" 返回"sdlfjd"
        /// </summary>
        /// <param name="strSrc"></param>
        /// <param name="strTrim"></param>
        /// <returns></returns>
        public static string TrimStart(string strSrc, string strTrim)
        {
            if (strSrc.StartsWith(strTrim))
            {
                string strDes = strSrc.Substring(strTrim.Length, strSrc.Length - strTrim.Length);
                return TrimStart(strDes, strTrim);
            }
            return strSrc;
        }
        /// <summary>
        /// 去除原字符串结尾处的所有替换字符串
        /// 即使是 TirmEnd("abcd".ToCharArray()) ，其作用也是删除字符（删除尾部出现的a或b或c或d字符，删除的过程直到碰到一个既不是a也不是b也不是c也不是d的字符才结束），而不是删除字符串“abcd”。
        /// 如：原字符串"sdlfjdcdcd",替换字符串"cd" 返回"sdlfjd"
        /// </summary>
        /// <param name="strSrc"></param>
        /// <param name="strTrim"></param>
        /// <returns></returns>
        public static string TrimEnd(string strSrc, string strTrim)
        {
            if (strSrc.EndsWith(strTrim))
            {
                string strDes = strSrc.Substring(0, strSrc.Length - strTrim.Length);
                return TrimEnd(strDes, strTrim);
            }
            return strSrc;
        }
        /// <summary>
        /// 取得字符串中数字部分
        /// </summary>
        /// <param name="str"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string OnlyNumbers(this string str, string input)
        {
            return new string(input.Where(char.IsDigit).ToArray());
        }
        /// <summary>
        /// 取得字符串中的汉字部分
        /// </summary>
        /// <param name="oriText"></param>
        /// <returns></returns>
        public static string GetChineseWord(string oriText)
        {
            string x = @"[\u4E00-\u9FFF]+";
            MatchCollection Matches = Regex.Matches
            (oriText, x, RegexOptions.IgnoreCase);
            StringBuilder sb = new StringBuilder();
            foreach (Match NextMatch in Matches)
            {
                sb.Append(NextMatch.Value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 字符串首字母转大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUpperFirst(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            char[] a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        /// <summary>
        /// 字符串首字母转小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToLowerFirst(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            char[] a = str.ToCharArray();
            a[0] = char.ToLower(a[0]);
            return new string(a);
        }

        /// <summary>
        /// 字符串首字母大写其余小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUpperFirstOtherLower(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            str = str.ToLower();
            char[] a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        /// <summary>
        /// 字符串里每个英文单词的首字母转为大写，假如”red house”会转换为 “Red House”
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// 提取两个指定字符串之间的字符串
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="startstr"></param>
        /// <param name="endstr"></param>
        /// <returns></returns>
        public static string MidStr_Start_End(string sourse, string startstr, string endstr)
        {
            Regex rg = new Regex("(?<=(" + startstr + "))[.\\s\\S]*?(?=(" + endstr + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(sourse).Value;
        }
        /// <summary>
        /// 提取两个指定字符串之间的字符串
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="startstr"></param>
        /// <param name="endstr"></param>
        /// <returns></returns>
        public static string ReplaceStr_Start_End(string sourse, string replacestr, string startstr, string endstr)
        {
            Regex rg = new Regex("(?<=(" + startstr + "))[.\\s\\S]*?(?=(" + endstr + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            var ret = rg.Replace(sourse, replacestr, 1); // 1为替换最大次数
            ret = ret.Replace(ret, startstr);
            ret = ret.Replace(ret, endstr);
            return ret;
        }

        /// <summary>
        /// 逐行读取，包含空行
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<string> SplitByLine(string text)
        {
            List<string> lines = new List<string>();
            byte[] array = Encoding.UTF8.GetBytes(text);
            using (MemoryStream stream = new MemoryStream(array))
            {
                using (var sr = new StreamReader(stream))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        lines.Add(line);
                        line = sr.ReadLine();
                    };
                }

            }
            return lines;
        }

    }
}
