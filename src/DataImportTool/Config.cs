using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImportTool
{
    class Config
    {
        // public static string DatabaseFile = @"F:\data\etoolkit\db\etk.dll";
        public static string DatabaseFile = @".\etk.dll";
        public static string ToolFile = @"F:\data\etoolkit\定版诊断软件\伍德沃德天然气OH6专用诊断工具-专业版.wtool";
        public static string SaveFile = @"F:\data\etoolkit\test\1.wtool";

        public static byte[][] codec = new byte[][]{
            new byte[]{87, 111, 111, 100, 119, 97, 114, 100},
            new byte[]{69, 84, 111, 111,108, 75, 105, 116}};

        public static string DataSource
        {
            get
            {
                return string.Format("data source={0}", DatabaseFile);
            }
        }
    }
}
