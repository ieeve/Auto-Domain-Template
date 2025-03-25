namespace Infrastructure.Common.Helpers
{
    /// <summary>
    /// 时间戳
    /// </summary>
    public class TimeHelper
    {
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string GetTimeStamp(DateTime time, int length = 13)
        {
            var ts = ConvertDateTimeToInt(time);
            return ts.ToString().Substring(0, length);
        }
        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(DateTime time)
        {
            var startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, 0), TimeZoneInfo.Local);
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }
        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name="timeStamp">时间戳</param>        
        /// <returns></returns>        
        public static DateTime ConvertStringToDateTime(string timeStamp)
        {
            var dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long lTime = long.Parse(timeStamp + "0000");
            var toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// 时间戳转为C#格式时间10位
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime GetDateTimeFrom1970Ticks(long timeStamp)
        {
            var dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return dtStart.AddSeconds(timeStamp);
        }

        /// <summary>
        /// 验证时间戳
        /// </summary>
        /// <param name="time">时间戳</param>
        /// <param name="interval">差值（分钟）</param>
        /// <returns></returns>
        public static bool IsTime(long time, double interval)
        {
            var dt = GetDateTimeFrom1970Ticks(time);
            //取现在时间
            var dt1 = DateTime.Now.AddMinutes(interval);
            var dt2 = DateTime.Now.AddMinutes(interval * -1);
            if (dt > dt2 && dt < dt1)
            {
                return true;
            }

            return false;
        }

    }
}
