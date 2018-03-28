using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                        string[] strArray = content.Split( "\n".ToCharArray());
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
            }
            if (string.IsNullOrEmpty(txtPhone.Text))
            {
                this.lblPhone.Content = "联系手机不能为空";
            }

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
            this.Close();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            this.tbAbout.IsSelected = true;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            this.tbUpate.IsSelected = true;
        }
    }
}
