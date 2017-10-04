using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Common
{
    public class DataConvert
    {
        /// <summary>
        /// 将value从string类型转换成int类型
        /// 转换成功，返回成功转换值，否则返回0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ConvertToIntEx(string value)
        {
            int result = default(Int32);
            Int32.TryParse(value, out result);
            return result;
        }

        /// <summary>
        /// 将value从string类型转换成int类型
        /// 转换成功，返回成功转换值，否则返回defaultValue
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ConvertToIntEx(string value,int defaultValue)
        {
            int result = default(Int32);
            if (Int32.TryParse(value, out result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// 获取当前时间 并转为int类型
        /// </summary>
        /// <returns></returns>
        public static int GetDateNowOfIntTime()
        {
            return Convert.ToInt16(Regex.Replace(DateTime.Now.ToShortTimeString(), ":", ""));
        }

        public static string GetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return value;
        }

        public static Tuple<string, string> GetDate(DateTime dateTime)
        {
            return Tuple.Create(dateTime.Date.ToString(),dateTime.Hour.ToString()+dateTime.Minute.ToString());
        }
    }
}
