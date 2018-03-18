using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataImportTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// TODO 导入文件
        /// （1）读取文件
        /// （2）分段截取，并对关键部分进行加密
        /// （3）分段保存到sqlite数据库中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            ReadFile(Config.ToolFile);
        }

        private void ReadFile(string filePath)
        {
            // read file and split by line
            DataAccess.open();
            byte[] fileBytes = File.ReadAllBytes(Config.ToolFile);
            byte[] oldstr = Config.codec[0];
            byte[] newstr = Config.codec[1];

            byte[] block1 = new byte[Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["num1"])];
            byte[] block2 = new byte[Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["num2"])];
            byte[] block3 = new byte[fileBytes.Length - (block1.Length + block2.Length)];
            System.Buffer.BlockCopy(fileBytes, 0, block1, 0, block1.Length);
            System.Buffer.BlockCopy(fileBytes, block1.Length, block2, 0, block2.Length);
            System.Buffer.BlockCopy(fileBytes, block1.Length + block2.Length, block3, 0, block3.Length);

            ReplaceBytes(block1, oldstr, newstr);
            ReplaceBytes(block2, oldstr, newstr);
            ReplaceBytes(block3, oldstr, newstr);

            DataAccess._insert("metafile", 
                new Dictionary<string, object> {
                    { "lid", 1 },
                    { "content", block2 },
                    { "create_time", DateTime.Now }
                });

            DataAccess._insert("metafile",
                new Dictionary<string, object> {
                    { "lid", 2 },
                    { "content", block1 },
                    { "create_time", DateTime.Now }
                });

            DataAccess._insert("data_content",
                new Dictionary<string, object> {
                    { "lid", 1 },
                    { "content", block3 },
                    { "create_time", DateTime.Now }
                });

            DataAccess.close();

        }

        /// <summary>
        /// TODO 从sqlite数据中读取文件，并合成到磁盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;
            DataAccess.open();
            List<DataContent> list1 = DataAccess.readall("select lid, content from metafile");
            List<DataContent> list2 = DataAccess.readall("select lid, content from data_content");
            DataAccess.close();
            DateTime end = DateTime.Now;
            TimeSpan ts = end - start;
            MessageBox.Show($"读取时间 {ts.TotalSeconds} 秒");


            start = DateTime.Now;

            byte[] oldstr = Config.codec[0];
            byte[] newstr = Config.codec[1];

            byte[] block1 = null, block2 = null, block3 = null;
            //read from db
            for ( int i = 0; i < list1.Count; i++ )
            {
                ReplaceBytes(list1[i].content, Config.codec[1], Config.codec[0]);
                if( list1[i].lid == 1)
                {
                    block2 = list1[i].content; 
                }
                else if (list1[i].lid == 2)
                {
                    block1 = list1[i].content;
                }
            }
            for (int i = 0; i < list2.Count; i++ )
            {
                ReplaceBytes(list2[i].content, Config.codec[1], Config.codec[0]);
                block3 = list2[i].content;
            }

            byte[] newBytes = new byte[block1.Length + block2.Length + block3.Length];

            System.Buffer.BlockCopy(block1, 0, newBytes, 0, block1.Length);
            System.Buffer.BlockCopy(block2, 0, newBytes, block1.Length, block2.Length);
            System.Buffer.BlockCopy(block3, 0, newBytes, block1.Length + block2.Length, block3.Length);
            File.WriteAllBytes(Config.SaveFile, newBytes);

            end = DateTime.Now;
            ts = end - start;
            MessageBox.Show($"写入时间 {ts.TotalSeconds} 秒");
        }

        /// <summary>
        /// TODO 测试，保留配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// TODO 通过ftp上传文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// TODO 提交用户信息，通过什么样的方式？
        /// 与上传账号是否关联？
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataAccess.passwd();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;
            DataAccess.open();
            List<DataContent> list = DataAccess.readall("select lid, content from data_content");
            DataAccess.close();
            DateTime end = DateTime.Now;
            TimeSpan ts = end - start;
            MessageBox.Show($"{list.Count}行 {ts.TotalSeconds} 秒");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            byte[] fileBytes = File.ReadAllBytes(Config.ToolFile);
            byte[] oldstr = Config.codec[0];
            byte[] newstr = Config.codec[1];

            byte[] block1 = new byte[fileBytes.Length / 100];
            byte[] block2 = new byte[fileBytes.Length / 100];
            byte[] block3 = new byte[fileBytes.Length - (block1.Length + block2.Length)];
            System.Buffer.BlockCopy(fileBytes, 0, block1, 0, block1.Length);
            System.Buffer.BlockCopy(fileBytes, block1.Length, block2, 0, block2.Length);
            System.Buffer.BlockCopy(fileBytes, block1.Length + block2.Length, block3, 0, block3.Length);

            ReplaceBytes(block1, oldstr, newstr);
            ReplaceBytes(block2, oldstr, newstr);
            ReplaceBytes(block3, oldstr, newstr);
            //write to db

            //read from db
            ReplaceBytes(block1, newstr, oldstr);
            ReplaceBytes(block2, newstr, oldstr);
            ReplaceBytes(block3, newstr, oldstr);

            byte[] allBytes = new byte[block1.Length + block2.Length + block3.Length];

            System.Buffer.BlockCopy(block1, 0, allBytes, 0, block1.Length);
            System.Buffer.BlockCopy(block2, 0, allBytes, block1.Length, block2.Length);
            System.Buffer.BlockCopy(block3, 0, allBytes, block1.Length + block2.Length, block3.Length);
            File.WriteAllBytes(Config.SaveFile, allBytes);

        }

        /// <summary>
        /// !!MUST: search.length == repl.length
        /// </summary>
        /// <param name="src"></param>
        /// <param name="search"></param>
        /// <param name="repl"></param>
        public void ReplaceBytes(byte[] src, byte[] search, byte[] repl)
        {
            for ( int i = 0; i < src.Length; i++ )
            {
                if (src[i] == search[0])
                {
                    if (i + search.Length < src.Length)
                    {
                        bool equal = true;
                        for( int x = 1; x < search.Length; x++ )
                        {
                            if (search[x] != src[i + x])
                            { equal = false; break; }
                        }

                        if (equal) // replace
                        {
                            for (int x = 0; x < repl.Length; x++)
                            {
                                src[i + x] = repl[x];
                            }
                            i = i + repl.Length - 1;
                        }
                    }
                }
            }
        }
        
    }
}
