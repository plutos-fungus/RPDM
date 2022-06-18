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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataRetriever dr = new();

        private static readonly string connStr = "Server=192.168.30.183;Database=ThisOne;User Id=SA;Password=!23456789ABc;MultipleActiveResultSets=True";

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
        }

        private void InsertData_Click(object sender, RoutedEventArgs e)
        {
            dr.DataInserter();
        }
    }
}
