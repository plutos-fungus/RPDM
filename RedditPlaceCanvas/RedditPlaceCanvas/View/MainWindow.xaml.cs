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
        private static readonly string connStr = "Server=192.168.30.183;Database=ThisOne;User Id=SA;Password=!23456789ABc";
        

        public MainWindow()
        {
            InitializeComponent();
        }

        private void PopularUser_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PopularColor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PopularCoord_Click(object sender, RoutedEventArgs e)
        {


        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Thread th1 = new(dr.DataReaderXArray);
            Thread th2 = new(dr.DataReaderYArray);
            Thread th3 = new(dr.DataReaderCoordArray);
            Thread th4 = new(dr.DataReaderColorList);
            Thread th5 = new(dr.DataReaderUserList);
            Thread th6 = new(dr.WhoPlacedMost);
            Thread th7 = new(dr.MostPopularColor);
            Thread th8 = new(dr.MostPopularCoord);

            th1.Start();
            th2.Start();
            th3.Start();
            th4.Start();
            th5.Start();
           
            th1.Join();
            th2.Join();
            th3.Join();
            th4.Join();
            th5.Join();

            th6.Start();
            th7.Start();
            th8.Start();

            th6.Join();
            th7.Join();
            th8.Join();

            CountedUsersListBox.ItemsSource = dr.countedUser;
            CountedColorListBox.ItemsSource = dr.countedColor;
            CountedCoordListBox.ItemsSource = dr.CountedCoords;

            dr.ClearList();

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
