using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var duration = "15 day 15day";
            var starDate = DateTime.Now;
            DurationToTimeSapn(duration, starDate, starDate);
        }
        public static bool DurationToTimeSapn(string duration, DateTime stopdate, DateTime startdate)
        {
            var result = new TimeSpan();
            var pattern = @"((?<indef>INDEF)|((?<firstTime>([0-9]+((minute(s)?|min)|(hours|hr)|(day)|(week(s)?|wk)|(month(s)?|mo)|(year(s)|yr))))(\s(?<secondTime>[0-9]+((minute(s)?|min)|(hours|hr)|day|(week(s)?|wk)|(month(s)?|mo)|(year(s)|yr)|(x|times))?)))|(?<indef>I))";
            var match = System.Text.RegularExpressions.Regex.Match(duration, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string key = match.Groups[1].Value;
                if (duration.Length != key.Length)
                {
                    return false;
                }
                var indef = match.Groups["indef"];
                var firstTime = match.Groups["firstTime"];
                var secondTime = match.Groups["secondTime"];
                if (indef != null)
                {
                    var old = DateTime.Parse("01/01/2070");
                    if (stopdate != old)
                        return false;
                    return true;
                }
                var patern2 = "(?<number>[0-9]+)((?<min>(minute(s)?|min))|(?<hour>(hours|hr))|(?<day>day)|(?<week>(week(s)?|wk))|(?<month>(month(s)?|mo))|(?<year>(year(s)|yr))";
                var number = 1;
                var min = 1;
                var hour = 1;
                var day = 1;
                var week = 1;
                var month = 1;
                //result.Days+=

            }
            return false;
        }
    }
}
