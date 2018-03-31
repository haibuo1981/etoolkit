using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EToolKit
{
    /// <summary>
    /// 考虑写入数据库或封装到c+ dll
    /// </summary>
    class Config
    {
        // public static string DatabaseFile = @"F:\data\etoolkit\db\etk.dll";
        public static string DatabaseFile = @"C:\tmp\etk.dll";
        public static string SaveFile = @"F:\data\etoolkit\test\1.wtool";
        public static string FtpServer = @"115.29.13.47";
        public static int FtpPort = 21;
        public static string FtpRoot = "/home/upload/test/";
        public static string user = "etoolkit_test";
        public static string pwd = "upload123";
        public static byte[][] codec = new byte[][]{
            new byte[]{87, 111, 111, 100, 119, 97, 114, 100},
            new byte[]{69, 84, 111, 111,108, 75, 105, 116}};

        public static bool IsPhoneNo(string str)
        {
            Regex regex = new Regex("^1[34578]\\d{9}$");
            Regex regex2 = new Regex("^\\(0\\d{2}\\)[- ]?\\d{8}$|^0\\d{2}[- ]?\\d{8}$|^\\(0\\d{3}\\)[- ]?\\d{7}$|^0\\d{3}[- ]?\\d{7}$");
            return regex.IsMatch(str) || regex2.IsMatch(str);
        }

        public static string DataSource
        {
            get
            {
                return string.Format("data source={0}", DatabaseFile);
            }
        }
    }
}
