using System;
using System.Collections.Generic;
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
using System.Drawing;
using RedditPlaceCanvas.Model;
using System.ComponentModel;
using System.Threading;
using System.Data.SqlClient;
using System.IO;

namespace RedditPlaceCanvas.View
{
    public struct StringSetter
    {
        public string Temp = new("");
        public int AllLines;
        public double Proc = 0;
        public string[] InserterArray;
        public double test = 0;
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataRetriever dr = new();
        StringSetter ss = new();
        private static readonly string ReadString = @"C:\Users\Rasmus\Desktop\Reddit_Project\RPDM\RedditPlaceCanvas\RedditPlaceCanvas\bin\Debug\net6.0-windows\02_header.txt";
        private static readonly string connStr = "";
        

        public MainWindow()
        {
            InitializeComponent();
        }

        private void PopularUser_Click(object sender, RoutedEventArgs e)
        {
            dr.WhoPlacedMost();
            CountedUsersListBox.ItemsSource = dr.countedUser;
        }

        private void PopularColor_Click(object sender, RoutedEventArgs e)
        {
            dr.MostPopularColor();
            CountedColorListBox.ItemsSource = dr.countedColor;
        }

        private void PopularCoord_Click(object sender, RoutedEventArgs e)
        {
            dr.MostPopularCoord();
            CountedCoordListBox.ItemsSource = dr.CountedCoords;
            dr.ClearList();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            dr.DataReader();
            
            //SettingsWindow sw = new();
            //sw.Show();
        }

        private void InsertData_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new();
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();
        }

        private void worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            InserterProgress.Value = e.ProgressPercentage;
        }

        // I know that you shouldn't have logic in the View layer.
        // But I had a hard time thinking of a better at the moment of writing.
        // I found this in a video by the YouTuber "Learn 2 Stop Hunger"
        // in the video "How To Use The Progress Bar In WPF For Long Running Tasks".
        // Thank you for making the video, I learned a lot : ).
        private void worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            worker.ReportProgress(0);

            using (SqlConnection conn = new(connStr))
            {
                conn.Open();
                ss.AllLines = File.ReadAllLines(ReadString).Count();
                foreach (string s in File.ReadLines(ReadString))
                {
                    ss.Temp = s;
                    ss.Temp = ss.Temp.Replace(" UTC", "");
                    ss.Temp = ss.Temp.Replace('"', ' ');
                    ss.InserterArray = ss.Temp.Split(',');
                    ss.InserterArray[3].Trim();
                    ss.InserterArray[4].Trim();

                    SqlCommand command = new("INSERT INTO RedditPlaces (PixelTime, UserUUID, Color, XCoord, YCoord) " + "VALUES (@pixelTime, @userUUID, @color, @xCoord, @yCoord)", conn);
                    command.Parameters.Add("@PixelTime", System.Data.SqlDbType.DateTime2).Value = DateTime.Parse(ss.InserterArray[0]);
                    command.Parameters.Add("@UserUUID", System.Data.SqlDbType.NVarChar).Value = ss.InserterArray[1].ToString();
                    command.Parameters.Add("@color", System.Data.SqlDbType.NVarChar).Value = ss.InserterArray[2].ToString();
                    command.Parameters.Add("@xCoord", System.Data.SqlDbType.Int).Value = ss.InserterArray[3];
                    command.Parameters.Add("@yCoord", System.Data.SqlDbType.NVarChar).Value = ss.InserterArray[4].ToString();
                    command.ExecuteNonQuery();
                    worker.ReportProgress(Convert.ToInt32(ss.test));
                    ss.test = (ss.Proc / ss.AllLines) * 100;
                    ss.Proc++;
                }
                worker.ReportProgress(100);
            }
        }

        private void worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("GG my guy");
            InserterProgress.Value = 0;
        }
    }
}
