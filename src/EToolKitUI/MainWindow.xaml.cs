using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EToolKit
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void invokeDelegate();
        static string ApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public MainWindow()
        {
            InitializeComponent();
            FullScreenManager.RepairWpfWindowFullScreenBehavior(this);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            exit();
        }

        private void exit()
        {
            if (proc != null)
            {
                try
                {
                    proc.CloseMainWindow();
                    proc.Close();
                }
                finally
                {

                }
                
            }
            
            this.Close();
        }

        private void maxButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void mniButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void menuButton_Click(object sender, RoutedEventArgs e)
        {
            Menu.IsOpen = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.btnSave.IsEnabled = true;
            this.txtName.IsReadOnly = false;
            this.txtPhone.IsReadOnly = false;
            this.txtCompany.IsReadOnly = false;
            this.txtAddress.IsReadOnly = false;
            this.txtName.Focus();
        }

        private void TabItem_TouchDown(object sender, TouchEventArgs e)
        {
            this.txtAddress.Text = "123";
        }

        private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.txtAddress.Text = "123";
        }

        private void TabItem_Loaded(object sender, RoutedEventArgs e)
        {
            Load_UserInfo();
        }

        private void Load_UserInfo()
        {
            // check file exists
            string dir = System.IO.Path.Combine(ApplicationData, "etoolkit");
            try
            {
                DirectoryInfo di = null;
                if (!Directory.Exists(dir))
                    di = Directory.CreateDirectory(dir);
                else
                    di = new DirectoryInfo(dir);

                if (di != null)
                {
                    string file_name = System.IO.Path.Combine(di.FullName, ".data");
                    if (File.Exists(file_name))
                    {
                        // File.SetAttributes(file_name, FileAttributes.Normal);
                        string str = File.ReadAllText(file_name);
                        string content = Protocal.Decrypt(str);
                        string[] strArray = content.Split("\n".ToCharArray());
                        if (strArray.Length == 4)
                        {
                            this.txtName.Text = strArray[0];
                            this.txtPhone.Text = strArray[1];
                            this.txtCompany.Text = strArray[2];
                            this.txtAddress.Text = strArray[3];
                        }


                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }
        }

        private void Label_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void lblVersion_Loaded(object sender, RoutedEventArgs e)
        {
            this.lblVersion.Content = "Version:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                this.lblName.Content = "联系人不能为空";
                return;
            }
            if (string.IsNullOrEmpty(txtPhone.Text) || !Config.IsPhoneNo(txtPhone.Text))
            {
                this.lblPhone.Content = "请输入正确的电话或手机号码";
                return;
            }

            this.lblName.Content = "";
            this.lblPhone.Content = "";

            string userName = this.txtName.Text;
            string phoneNum = this.txtPhone.Text;

            string company = string.IsNullOrEmpty(this.txtCompany.Text) ? "" : this.txtCompany.Text;
            string address = string.IsNullOrEmpty(this.txtAddress.Text) ? "" : this.txtAddress.Text;

            string dir = System.IO.Path.Combine(ApplicationData, "etoolkit");
            try
            {
                DirectoryInfo di = null;
                if (!Directory.Exists(dir))
                    di = Directory.CreateDirectory(dir);
                else
                    di = new DirectoryInfo(dir);

                if (di != null)
                {
                    string file_name = System.IO.Path.Combine(di.FullName, ".data");

                    if (File.Exists(file_name))
                    {
                        File.SetAttributes(file_name, FileAttributes.Normal);
                    }
                    File.WriteAllText(file_name,
                        Protocal.Encrypt($"{userName}\n{phoneNum}\n{company}\n{address}"));
                    File.SetAttributes(file_name, FileAttributes.System | FileAttributes.Hidden);

                    this.lblStatus.Content = "Write done";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }

            this.btnSave.IsEnabled = false;
            this.txtName.IsReadOnly = true;
            this.txtPhone.IsReadOnly = true;
            this.txtCompany.IsReadOnly = true;
            this.txtAddress.IsReadOnly = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            exit();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            this.tbAbout.IsSelected = true;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            this.tbUpate.IsSelected = true;
        }

        List<string> uploadList = new List<string>();

        /// <summary>
        /// 打开文件、或文件列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "etoolkit数据文件"; // Default file name
            dlg.DefaultExt = ".wset"; // Default file extension
            dlg.Filter = "etoolkit数据文件 (.wset)|*.wset"; // Filter files by extension
            dlg.Multiselect = true;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                if (uploadList.Count > 0)
                {
                    ShowLog($"清除历史队列");
                    this.uploadList.Clear();
                }


                this.uploadList.AddRange(dlg.FileNames);
                foreach (string filename in dlg.FileNames)
                {
                    ShowLog($"{filename} 添加到上传队列");

                }
            }



        }

        private void ShowLog(string msg, bool isWarn = false)
        {
            invokeDelegate del = () =>
            {
                if (isWarn)
                {
                    TextRange range = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd);
                    range.Text = $"Warn:\t{msg}\t{DateTime.Now.ToLongTimeString()}\n";
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);

                }
                else
                {
                    TextRange range = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd);
                    range.Text = $"{msg}\t{DateTime.Now.ToLongTimeString()}\n";
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);
                }
            };

            this.Dispatcher.Invoke(del);
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            // check the uploadList
            if (uploadList.Count == 0)
            {
                ShowLog("上传队列不能为空!", true);
                return;
            }

            // check the phone num is ok
            if (string.IsNullOrEmpty(txtPhone.Text))
            {
                ShowLog("亲爱的用户，我们会在上传数据后与您取得联系并确认具体情况，请确认已经正确填写用户信息!", true);
                return;
            }

            // check user_file exist
            string dir = System.IO.Path.Combine(ApplicationData, "etoolkit");
            string file_name = System.IO.Path.Combine(dir, ".data");

            if (!File.Exists(file_name))
            {
                ShowLog("亲爱的用户，我们需要用户信息以便与您联系，请确认已经保存用户信息!", true);
                return;
            }

            FTPClient ftp = new FTPClient(Config.FtpServer, Config.FtpRoot, Config.user, Config.pwd, Config.FtpPort);
            try
            {
                ftp.Connect();
            }
            catch (Exception ex)
            {
                ShowLog($"连接服务器异常，错误信息：{ex.Message}", true);
                return;
            }

            if (ftp.Connected)
            {
                invokeDelegate del = () =>
                {
                    ShowLog($"连接成功，正在为用户初始化上传目录...");
                };
                this.Dispatcher.Invoke(del);
                try
                {
                    string folder = txtPhone.Text;
                    ftp.MkDir(folder);
                    ftp.ChDir($"{Config.FtpRoot}{folder}");
                    ftp.Put(file_name);
                }
                catch (Exception ex)
                {
                    ShowLog($"初始化上传目录异常，请联系管理员。异常信息：{ex.Message}", true);
                    return;
                }
                finally
                {
                    invokeDelegate fin = () =>
                    {
                        ShowLog("初始化完成");
                    };
                    this.Dispatcher.Invoke(fin);
                }

                for (int i = uploadList.Count - 1; i >= 0; i--)
                {
                    string datafile = uploadList[i];
                    UploadObject obj = new UploadObject(ftp, datafile);

                    Thread th = new Thread(upload);
                    th.IsBackground = true;
                    th.Start(obj);
                }



            }


        }

        private void upload(object o)
        {
            UploadObject uploadObj = (UploadObject)o;
            string datafile = uploadObj.upload_file_name;
            FTPClient ftp = uploadObj.ftp;

            invokeDelegate start = () =>
            {
                ShowLog($"开始上传文件：{datafile}");
            };
            this.Dispatcher.Invoke(start);
            try
            {
                ftp.Put(datafile);
                invokeDelegate suc = () =>
                {
                    ShowLog($"上传成功：{datafile}");
                };
                this.Dispatcher.Invoke(suc);
                this.uploadList.Remove(datafile);
            }
            catch (Exception ex)
            {
                invokeDelegate con = () =>
                {
                    ShowLog($"上传文件{datafile}出错，请稍后再试。异常信息：{ex.Message}", true);
                };
                this.Dispatcher.Invoke(con);
            }
            finally
            {
                invokeDelegate fin2 = () =>
                {
                    ShowLog("完成");

                    if (uploadList.Count == 0)
                    {
                        ShowLog("全部上传完成");
                        ftp.DisConnect();
                    }
                };
                this.Dispatcher.Invoke(fin2);
            }


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "etoolkit工具包文件"; // Default file name
            dlg.DefaultExt = ".wset"; // Default file extension
            dlg.Filter = "etoolkit工具包文件 (.epkg)|*.epkg"; // Filter files by extension
            dlg.Multiselect = false;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                MessageBox.Show("无法识别的工具包文件，请确认导入的是合法的etoolkit工具包。", "提示");
            }
        }

        /// <summary>
        /// 提取文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (!CheckEnvironment())
            {
                return;
            }

            ReadFile();
            LoadHistoryDB();
            WriteConfig();
            StartTool(this.btnTool1.Content.ToString());
            UpdateCache();
        }

        private void UpdateCache()
        {
            if(File.Exists(tool_file_name))
            {
                try
                {
                    File.Delete(tool_file_name);
                }
                finally
                {

                }
            }
        }

        private bool CheckEnvironment()
        {
            Process[] processes = Process.GetProcesses();
            
            foreach(Process prc in processes)
            {
                if(prc.ProcessName.ToLower().Contains("toolkit") && !prc.ProcessName.ToLower().Contains("etoolkit"))
                {
                    MessageBoxResult dr = MessageBox.Show("检测到toolkit在运行中，运行本工具包会与之冲突，运行前必须关闭现有的程序。是否现在关闭？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (dr == MessageBoxResult.OK)
                    {
                        prc.CloseMainWindow();
                        prc.Close();
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            
            return true;
            
            //Microsoft.Win32.RegistryKey uninstallNode = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            //foreach (string subKeyName in uninstallNode.GetSubKeyNames())
            //{
            //    Microsoft.Win32.RegistryKey subKey = uninstallNode.OpenSubKey(subKeyName);
            //    object displayName = subKey.GetValue("DisplayName");
            //    if (displayName != null)
            //    {
            //        MessageBox.Show(displayName.ToString());
            //        if (displayName.ToString().Contains("Woodward ToolKit"))
            //        {
            //            return true;
                          
            //        }
            //    }
            //}
            //return false;
        }

        private void WriteConfig()
        {
            FileInfo fi = new FileInfo(tool_file_name);

            string[] lines = new string[2];
            lines[0] = fi.Name.Substring(1, 6);
            lines[1] = DateTime.Now.Ticks.ToString();

            File.WriteAllLines(config_file_name, lines);
        }

        string config_file_name = @".\etoolkit.config";

        private void LoadHistoryDB()
        {
            string[] configs = File.ReadAllLines(config_file_name);
            if (configs == null || configs.Length != 2)
            {
                return;
            }

            string filename = $"_{configs[0]}.wtool";
            FileInfo fi = new FileInfo(tool_file_name);

            string prop_file_path = System.IO.Path.Combine(new string[] 
            {
                $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}",
                "Woodward", "ToolKit", "Application.properties"
            });

            byte[] content = File.ReadAllBytes(prop_file_path);
            ReplaceBytes(content, System.Text.Encoding.Default.GetBytes(filename),
                System.Text.Encoding.Default.GetBytes(fi.Name));

            File.WriteAllBytes(prop_file_path, content);

        }

        string tool_file_name = "";

        Process proc = null;

        private void StartTool(string toolName)
        {
            if (!File.Exists(tool_file_name))
            {
                MessageBox.Show("打开工具包文件异常，请与管理联系。", "错误");
                return;
            }
            FileInfo file = new FileInfo(tool_file_name);

            ProcessStartInfo psi = new ProcessStartInfo(tool_file_name);

            // psi.UseShellExecute = false;
            // psi.FileName = @"c:\tmp\demo\OH6.Wtool";
            psi.WorkingDirectory = @".\";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.WindowStyle = ProcessWindowStyle.Minimized;
            proc = Process.Start(psi);


            int loop = 6000;
            while (true)
            {
                IntPtr ptr = MyTool.FindWindow(null, $"{file.Name} - Woodward ToolKit");
                if (ptr != IntPtr.Zero)
                {
                    MyTool.SetWindowText(ptr, toolName);

                    // MyTool.SetForegroundWindow(proc.MainWindowHandle);
                    MyTool.ShowWindowAsync(proc.MainWindowHandle, 3);

                    break;
                }
                else
                {
                    IntPtr ptr2 = MyTool.FindWindow(null, "Woodward ToolKit");
                    if (ptr != IntPtr.Zero)
                    {
                        MyTool.SetWindowText(ptr2, "EToolKit");
                    }
                }
                loop--;
                if (loop < 0)
                    break;
                else
                {
                    System.Threading.Thread.Sleep(10);
                }
            }
        }

        private void ReadFile()
        {
            DataAccess.open();
            List<DataContent> list1 = DataAccess.readall("select lid, content from metafile");
            List<DataContent> list2 = DataAccess.readall("select lid, content from data_content");
            DataAccess.close();

            byte[] oldstr = Config.codec[0];
            byte[] newstr = Config.codec[1];

            byte[] block1 = null, block2 = null, block3 = null;
            //read from db
            for (int i = 0; i < list1.Count; i++)
            {
                ReplaceBytes(list1[i].content, Config.codec[1], Config.codec[0]);
                if (list1[i].lid == 1)
                {
                    block2 = list1[i].content;
                }
                else if (list1[i].lid == 2)
                {
                    block1 = list1[i].content;
                }
            }
            for (int i = 0; i < list2.Count; i++)
            {
                ReplaceBytes(list2[i].content, Config.codec[1], Config.codec[0]);
                block3 = list2[i].content;
            }

            byte[] newBytes = new byte[block1.Length + block2.Length + block3.Length];

            System.Buffer.BlockCopy(block1, 0, newBytes, 0, block1.Length);
            System.Buffer.BlockCopy(block2, 0, newBytes, block1.Length, block2.Length);
            System.Buffer.BlockCopy(block3, 0, newBytes, block1.Length + block2.Length, block3.Length);

            string dir = System.IO.Path.Combine(ApplicationData, "micro_soft");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            Random r = new Random((int)DateTime.Now.Ticks);

            string filename = $"_{r.Next(800000, 888888)}.wtool";
            tool_file_name = System.IO.Path.Combine(dir, filename);
            try
            {
                File.WriteAllBytes(tool_file_name, newBytes);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }

        /// <summary>
        /// !!MUST: search.length == repl.length
        /// </summary>
        /// <param name="src"></param>
        /// <param name="search"></param>
        /// <param name="repl"></param>
        public void ReplaceBytes(byte[] src, byte[] search, byte[] repl)
        {
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] == search[0])
                {
                    if (i + search.Length < src.Length)
                    {
                        bool equal = true;
                        for (int x = 1; x < search.Length; x++)
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


        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("此工具包内容没有解锁", "提示");
        }
    }
}
