using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            comments.Clear();
            String idGroup = txtIdGroup.Text;
            String idPost = txtIdPost.Text;
            if(String.IsNullOrEmpty(idPost))
            {
                dialog.BringIntoView();
                dialog.Visibility = Visibility.Visible;
                return;
            }
            String post_id = String.IsNullOrEmpty(idGroup) ? idPost : idGroup + "_" + idPost;
            ChromeController chromeController = new ChromeController();
            chromeController.CallBack = Callback;
            chromeController.CallBackError = CallbackError;
            chromeController.CallBackEnd = CallbackSuccess;
            chromeController.init(@"C:\Users\MinhHoangJSC\AppData\Local\Google\Chrome\User Data", @"C:\Program Files\Google\Chrome\Application\chrome.exe");
            chromeController.ChromeDriver.Navigate().GoToUrl("https://www.facebook.com/");
            chromeController.getListComment(1000, post_id);
        }

        public void Callback(Object data)
        {
            JObject commentData = (JObject)data;
            JArray commentInfo = JArray.FromObject(commentData["data"]["node"]["display_comments"]["edges"]);
            foreach( JObject cmt in commentInfo)
            {
                Comment comment = new Comment();
                comment.Id = cmt["node"]["legacy_token"].ToString();
                comment.Create_at = new DateTime(long.Parse(cmt["node"]["created_time"].ToString()));
                comment.Message = cmt["node"]["body"].HasValues ? cmt["node"]["body"]["text"].ToString() : "";
                comment.Username_comment = cmt["node"]["author"]["name"].ToString();
                comment.Link_user_comment = cmt["node"]["author"]["url"].ToString();
                this.Dispatcher.Invoke(() =>
                {
                    comments.Add(comment);
                });
               
            }
            
        }
        public void CallbackSuccess(Object data)
        {
            JArray dataArrr = (JArray)data;
            MessageBox.Show("Done: "+dataArrr.Count);
        }
        public void CallbackError(Object data)
        {

        }
        
        private void btnCancelDialog_Click(object sender, RoutedEventArgs e)
        {
            dialog.Visibility = Visibility.Collapsed;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = txtSearch.Text;
            comments = new ObservableCollection<Comment>((from item in comments where item.Message.Contains(search) select item).ToList());
        }
    }
}
