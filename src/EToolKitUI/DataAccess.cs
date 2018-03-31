using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Reflection;

namespace EToolKit
{
    class DataAccess
    {
        static SQLiteConnection _conn;
        DataAccess()
        {

        }

        public static void passwd()
        {
            _conn = new SQLiteConnection(Config.DataSource);
            _conn.Open();
            _conn.ChangePassword("kj4Dfj&1");
        }
        public static void open()
        {
            _conn = new SQLiteConnection(Config.DataSource);
            _conn.SetPassword("kj4Dfj&1");
            _conn.Open();
        }

        public static void close()
        {
            _conn.Close();
        }

        

        public static void insert(string sql)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Config.DataSource))
            {
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    sh.Execute(sql);
                    conn.Close();
                }
                
            }
        }

        public static void insert(string table, Dictionary<string, object> dict) {
            using (SQLiteConnection conn = new SQLiteConnection(Config.DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    
                    sh.Insert(table, dict);

                    conn.Close();

                }
            }
        }

        public static void _insert(string table, Dictionary<string, object> dict)
        {
            using (SQLiteCommand cmd = _conn.CreateCommand())
            {
   
                SQLiteHelper sh = new SQLiteHelper(cmd);

                sh.Insert(table, dict);

            }
        }

        public static List<DataContent> readall(string sql)
        {
            List<DataContent> list = new List<DataContent>();

            using (SQLiteCommand cmd = _conn.CreateCommand())
            {
                SQLiteHelper sh = new SQLiteHelper(cmd);
                DataTable dt = sh.Select(sql);
                list = ConvertHelper<DataContent>.ConvertToList(dt);
            }
          

            return list;

        }
        

    }


    class ConvertHelper<T> where T : new()
    {
        /// <summary>
        /// 利用反射和泛型
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ConvertToList(DataTable dt)
        {

            // 定义集合
            List<T> ts = new List<T>();

            // 获得此模型的类型
            Type type = typeof(T);
            //定义一个临时变量
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量
                    //检查DataTable是否包含此列（列名==对象的属性名）  
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter
                        if (!pi.CanWrite) continue;//该属性不可写，直接跳出
                        //取值
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性
                        if (value != DBNull.Value)
                        {
                            if (pi.GetMethod.ReturnParameter.ParameterType.Name == "Int32")
                            {
                                value = Convert.ToInt32(value);
                            }
                            pi.SetValue(t, value, null);
                        }

                    }
                }
                //对象添加到泛型集合中
                ts.Add(t);
            }

            return ts;

        }
    }
}
