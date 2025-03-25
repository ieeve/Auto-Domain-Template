using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Common.Utility
{
    public class HtmlHelper
    {

        //URL转硬盘路径
        public static string UrlPath2DiskPath(string urlPath)
        {
            if (string.IsNullOrWhiteSpace(urlPath)) return string.Empty;

            List<string> path = new List<string>
            {
                AppContext.BaseDirectory
            };

            var path_arr = urlPath.Split('/');
            foreach (var item in path_arr)
            {
                //if (!string.IsNullOrWhiteSpace(item) && !item.Contains('.')) //不包括文件
                if (!string.IsNullOrWhiteSpace(item))
                {
                    path.Add(item);
                }
            }
            var DiskDirPath = Path.Combine(path.ToArray());
            return DiskDirPath;
        }
        //除去所有在html元素中标记
        public static string striphtml(string strhtml)
        {
            string stroutput = strhtml;
            Regex regex = new Regex(@"<[^>]+>|</[^>]+>");

            stroutput = regex.Replace(stroutput, "");
            return stroutput;

        }

        /// <summary>
        ///  从html中提取纯文本
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public static string getTxtFromHtml(string strHtml)
        {
            if (string.IsNullOrWhiteSpace(strHtml)) return string.Empty;
            Regex regex = new Regex("<.+?>", RegexOptions.IgnoreCase);
            string strOutput = regex.Replace(strHtml, "");//替换掉"<"和">"之间的内容
            strOutput = strOutput.Replace("<", "");
            strOutput = strOutput.Replace(">", "");
            strOutput = strOutput.Replace("&nbsp;", "");
            return strOutput;
        }

        /// <summary>
        ///  从html中提取纯文本
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public static string getTxtFromHtml(string strHtml, int top)
        {
            var strOutput = getTxtFromHtml(strHtml);
            if (!string.IsNullOrWhiteSpace(strOutput))
            {
                if (strOutput.Length > top) strOutput = strOutput.Substring(top) + "...";
            }
            return strOutput;
        }

        //在C#后台实现JavaScript的函数escape()的字符串转换
        //些方法支持汉字
        public static string Escape(string s)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byteArr = System.Text.Encoding.Unicode.GetBytes(s);

            for (int i = 0; i < byteArr.Length; i += 2)
            {
                sb.Append("%u");
                sb.Append(byteArr[i + 1].ToString("X2"));//把字节转换为十六进制的字符串表现形式

                sb.Append(byteArr[i].ToString("X2"));
            }
            return sb.ToString();

        }

        //把JavaScript的escape()转换过去的字符串解释回来
        //些方法支持汉字
        public static string Unescape(string s)
        {

            string str = s.Remove(0, 2);//删除最前面两个＂%u＂
            string[] strArr = str.Split(new string[] { "%u" }, StringSplitOptions.None);//以子字符串＂%u＂分隔
            byte[] byteArr = new byte[strArr.Length * 2];
            for (int i = 0, j = 0; i < strArr.Length; i++, j += 2)
            {
                byteArr[j + 1] = Convert.ToByte(strArr[i].Substring(0, 2), 16);  //把十六进制形式的字串符串转换为二进制字节
                byteArr[j] = Convert.ToByte(strArr[i].Substring(2, 2), 16);
            }
            str = System.Text.Encoding.Unicode.GetString(byteArr); //把字节转为unicode编码
            return str;

        }

        /// <summary> 
        /// 取得HTML中所有图片的 URL。 
        /// </summary> 
        /// <param name="sHtmlText">HTML代码</param> 
        /// <returns>图片的URL列表</returns> 
        public static string[] GetHtmlImageList(string sHtmlText)
        {
            // 定义正则表达式用来匹配 img 标签 
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串 
            MatchCollection matches = regImg.Matches(sHtmlText);
            int i = 0;
            string[] sUrlList = new string[matches.Count];

            // 取得匹配项列表 
            foreach (Match match in matches)
                sUrlList[i++] = match.Groups["imgUrl"].Value;
            return sUrlList;
        }
    }
}
