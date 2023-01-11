using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp2.controller;
using WpfApp2.model;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Groups> groups = new ObservableCollection<Groups>();
        ObservableCollection<Comment> comments = new ObservableCollection<Comment>();
        ChromeController chromeController = new ChromeController();
        public MainWindow()
        {
            InitializeComponent();
            
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Binding groupsBind = new Binding()
            {
                Source = groups
            };
            listViewGroups.SetBinding(ListView.ItemsSourceProperty, groupsBind);
            Binding commentBind = new Binding()
            {
                Source = comments
            };
            listViewComment.SetBinding(ListView.ItemsSourceProperty, commentBind);
            String user_path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            txtProfilePath.Text = user_path + @"\AppData\Local\Google\Chrome\User Data";
            txtChromePath.Text = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            comments.Clear();
            prComment.Value = 0;
            prComment.Maximum = 0;
            lbQuatity.Content = "";
            lbTotal.Content = "";
            String idGroup = txtIdGroup.Text;
            String idPost = txtIdPost.Text;
            if(String.IsNullOrEmpty(idPost) || String.IsNullOrEmpty(txtProfilePath.Text)|| String.IsNullOrEmpty(txtChromePath.Text))
            {
                ShowDialog("Data is Empty", "error");
                return;
            }
            String post_id = String.IsNullOrEmpty(idGroup) ? idPost : idGroup + "_" + idPost;
            
            chromeController.CallBack = Callback;
            chromeController.CallBackError = CallbackError;
            chromeController.CallBackEnd = CallbackSuccess;
            chromeController.init(txtProfilePath.Text,txtChromePath.Text);
            chromeController.getListComment(1000, post_id);
        }

        public void Callback(Object data)
        {
            JObject commentData = (JObject)data;
            JArray commentInfo = JArray.FromObject(commentData["data"]["node"]["display_comments"]["edges"]);

            this.Dispatcher.Invoke(() =>
            {
                prComment.Maximum = int.Parse(commentData["data"]["node"]["display_comments"]["count"].ToString());
            });
            foreach ( JObject cmt in commentInfo)
            {
                Comment comment = new Comment();
                comment.Id = cmt["node"]["legacy_token"].ToString();
                comment.Create_at = UnixTimeStampToDateTime(long.Parse(cmt["node"]["created_time"].ToString())).ToString();
                comment.Message = cmt["node"]["body"].HasValues ? cmt["node"]["body"]["text"].ToString() : "";
                comment.Username_comment = cmt["node"]["author"]["name"].ToString();
                comment.Link_user_comment = cmt["node"]["author"]["url"].ToString();
                comment.Link_comment = "https://www.facebook.com/" + comment.Id;
                this.Dispatcher.Invoke(() =>
                {
                    
                    comments.Add(comment);
                });
               
            }
            this.Dispatcher.Invoke(() =>
            {
                lbQuatity.Content = comments.Count + "";
                lbTotal.Content = commentData["data"]["node"]["display_comments"]["count"].ToString();
                prComment.Value = comments.Count;
                prComment.Maximum = int.Parse(commentData["data"]["node"]["display_comments"]["count"].ToString());
            });
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        public void CallbackSuccess(Object data)
        {

            ShowDialog("Done: "+comments.Count);
            
        }
        public void CallbackError(Object data)
        {

        }
        
        private void btnCancelDialog_Click(object sender, RoutedEventArgs e)
        {
            dialog.Visibility = Visibility.Collapsed;
        }
        ObservableCollection<Comment> commSearch;
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        public void ExportExcel(List<Comment> data)
        {
            string filePath = "";
            SaveFileDialog dialog = new SaveFileDialog();

            // chỉ lọc ra các file có định dạng Excel
            dialog.Filter = "Excel | *.xlsx | Excel 2003 | *.xls";

            if (dialog.ShowDialog() == true)
            {
                filePath = dialog.FileName;
            }
            if (string.IsNullOrEmpty(filePath))
            {
                ShowDialog("Đường dẫn báo cáo không hợp lệ", "error");
                return;
            }

            try
            {
                using (ExcelPackage p = new ExcelPackage())
                {
                    p.Workbook.Properties.Author = "Kit502";

                    p.Workbook.Properties.Title = "Comment";

                    p.Workbook.Worksheets.Add("Comment");

                    ExcelWorksheet ws = p.Workbook.Worksheets[0];

                    ws.Name = "Comment";
                    ws.Cells.Style.Font.Size = 11;
                    ws.Cells.Style.Font.Name = "Calibri";

                    ws.Cells["A1"].LoadFromCollection(data);

                    Byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);
                }
                ShowDialog("Xuất excel thành công!");
            }
            catch (Exception EE)
            {
                ShowDialog("Có lỗi khi lưu file!", "error");
            }
        }
        //3878010902480177
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if(commSearch == null)
                commSearch = new ObservableCollection<Comment>(comments);
            ExportExcel(commSearch.ToList());
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                string search = txtSearch.Text;
                commSearch = new ObservableCollection<Comment>((from item in comments where item.Message.Contains(search) select item).ToList());

                Binding commentBind = new Binding()
                {
                    Source = commSearch
                };
                listViewComment.SetBinding(ListView.ItemsSourceProperty, commentBind);
            }
            
        }
        // mở file excel

        private void ShowDialog(String message, String type = "success")
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (type)
                {
                    case "error":
                        iconType.Foreground = new SolidColorBrush(Colors.Red);
                        iconType.Kind = MaterialDesignThemes.Wpf.PackIconKind.Error;
                        lbType.Content = "Error:";
                        break;
                    default:
                        iconType.Foreground = new SolidColorBrush(Colors.Green);
                        iconType.Kind = MaterialDesignThemes.Wpf.PackIconKind.Success;
                        lbType.Content = "Success:";
                        break;
                }

                lbMessage.Content = message;
                dialog.Visibility = Visibility;
            });
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
            if(openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtProfilePath.Text = openFileDialog.SelectedPath;
            }
        }

        private void btnChrome_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtProfilePath.Text = openFileDialog.FileName;
            }
        }
    }
    
}
