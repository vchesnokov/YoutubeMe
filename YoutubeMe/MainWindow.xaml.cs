using System;
using System.Collections.Generic;
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

namespace YoutubeMe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private YoutubeGetMe youtubeGetMe = null;

        const string LOG_FILE = "log.txt";

        public MainWindow()
        {
            InitializeComponent();

            using (FileStream fs = File.Create(LOG_FILE)) { }

            lbLogInfo.Content = $"Смотрите ЖУРНАЛ действий в файле: {LOG_FILE}";

            youtubeGetMe = new YoutubeGetMe();
            youtubeGetMe.PushEvent += YoutubeGetMe_PushEvent;
            youtubeGetMe.PushVideoCountEvent += YoutubeGetMe_PushVideoCountEvent;

        }

        private void YoutubeGetMe_PushVideoCountEvent(object sender, EventArgs e)
        {
            YoutubeMeDownloadEventArgs ea = (YoutubeMeDownloadEventArgs)e;

            lbVideoCount.Content = $"Найдено видео (шт.): {ea.Message}";
        }

        private void YoutubeGetMe_PushEvent(object sender, EventArgs e)
        {
            YoutubeMeDownloadEventArgs ea = (YoutubeMeDownloadEventArgs)e;

            addToLog(ea.Message);
        }

        private void addToLog(string message)
        {
            //txtLog.Text += "\r\n" + message;
            txtLog.Items.Add("\r\n" + message);

            AppendTextToLog(message);
        }

        private void AppendTextToLog(string message)
        {
            using (StreamWriter w = File.AppendText(LOG_FILE))
            {
                w.WriteLine(message);
            }
        }

        private void btnGetList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = txtYoutubeAddress.Text;
                //int intOrderVideoNumber = 1;
                //intOrderVideoNumber = int.Parse(txtOrderVideoNumber.Text);

                youtubeGetMe.GetList(url); ///, intOrderVideoNumber);

                btnDownload.IsEnabled = true;
                //youtubeGetMe.DownloadList();

                //MessageBox.Show("btnGetList_Click");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int intOrderVideoNumber = 1;
                intOrderVideoNumber = int.Parse(txtOrderVideoNumber.Text);

                youtubeGetMe.DownloadList(intOrderVideoNumber);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
