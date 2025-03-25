namespace Infrastructure.Common.Helpers
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// 取得开始时间和结束时间（上月26号-当月25号）
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static (DateTime start, DateTime end) CalStartTiem_EndTime(int year, int month)
        {
            if (year == 0) year = DateTime.Now.Year;
            if (month == 0) month = DateTime.Now.Month;

            var TempTime = DateTime.Parse(year + "-" + month + "-" + DateTime.Now.Day);
            //如果超过25号，那么计算下个月的
            var CurrentTime = TempTime.Day > 25 ? TempTime.AddMonths(1) : TempTime;

            var EndTime = DateTime.Parse(year + "-" + month + "-25");
            var Temp = DateTime.Parse(year + "-" + month + "-26"); //零点零分
            var StartTime = Temp.AddMonths(-1);//如果当月是1月份，上月为去年12月

            return (StartTime, EndTime);
        }

        public static (DateTime start, DateTime end) CalCurrentStartTiem_EndTime()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            return CalStartTiem_EndTime(year, month);
        }

        /// <summary>
        /// 计算两个时间中有多少年份
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<int> GetYearsByStart_End(DateTime start, DateTime end)
        {
            var StartYear = start.Year;
            var EndYear = end.Year;
            List<int> Years = new List<int>();
            while (StartYear <= EndYear)
            {
                Years.Add(StartYear);
                StartYear++;
            }
            return Years;
        }
    }
}
