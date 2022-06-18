using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace RedditPlaceCanvas.Model
{
    public struct SQLData
    {
        public List<String> ColorList = new();
        //public List<int> XCoord = new();
        //public List<int> YCoord = new();
        //public List<string> Coords = new();
        public List<string> User = new();

        
    }

    public class DataRetriever
    {
        SQLData sd = new();
        private static readonly string connStr = "Server=192.168.30.183;Database=ThisOne;User Id=SA;Password=!23456789ABc";
        private static readonly string ReadString = @"C:\Users\Rasmus\Desktop\Reddit_Project\RPDM\RedditPlaceCanvas\RedditPlaceCanvas\bin\Debug\net6.0-windows\oof.txt";
        //public List<String> ColorList = new();
        //public List<int> XCoord = new();
        //public List<int> YCoord = new();
        //public List<string> Coords = new();
        //public List<string> User = new();

        public int j = 0;
        public int[]? XArray;
        public int[]? YArray;
        public string[]? Coords;


        public ObservableCollection<string> CountedCoords = new();
        public ObservableCollection<string> countedUser = new();
        public ObservableCollection<string> countedColor = new();

        public DataRetriever()
        {
            using (SqlConnection conn = new(connStr))
            {
                conn.Open();
                SqlCommand command = new("SELECT Id FROM RedditPlaces ORDER BY Id DESC;", conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        for (int i = 0; i < 1; i++)
                        {
                            j = Convert.ToInt32(reader["Id"]);
                        }
                    }
                }
            }
            XArray = new int[j];
            YArray = new int[j];
            Coords = new string[j];
        }

        public void DataReaderXArray()
        {
            int i = 0;
            using (SqlConnection conn = new(connStr))
            {
                conn.Open();
                SqlCommand command = new("SELECT XCoord FROM RedditPlaces ORDER BY Id DESC;", conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        XArray[i] = Convert.ToInt32(reader["XCoord"]);
                        i++;
                    }
                }
            }
        }

        public void DataReaderYArray()
        {
            int i = 0;
            using (SqlConnection conn = new(connStr))
            {
                conn.Open();
                SqlCommand command = new("SELECT YCoord FROM RedditPlaces ORDER BY Id DESC;", conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        YArray[i] = Convert.ToInt32(reader["YCoord"]);
                        i++;
                    }
                }
            }
        }

        public void DataReaderCoordArray()
        {
            int i = 0;
            using (SqlConnection conn = new(connStr))
            {
                conn.Open();
                SqlCommand command = new("SELECT XCoord, YCoord FROM RedditPlaces ORDER BY Id DESC;", conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Coords[i] = Convert.ToString(reader["XCoord"]) + " " + Convert.ToString(reader["YCoord"]);
                        i++;
                    }
                }
            }
        }

        public void DataReaderColorList()
        {
            using (SqlConnection conn = new(connStr))
            {
                conn.Open();
                SqlCommand command = new("SELECT Color FROM RedditPlaces ORDER BY Id DESC;", conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sd.ColorList.Add(Convert.ToString(reader["Color"]));
                    }
                }
            }
        }

        public void DataReaderUserList()
        {
            using (SqlConnection conn = new(connStr))
            {
                conn.Open();
                SqlCommand command = new("SELECT UserUUID FROM RedditPlaces ORDER BY Id DESC;", conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sd.User.Add(Convert.ToString(reader["UserUUID"]));
                    }
                }
            }
        }

        public void WhoPlacedMost()
        {
            // I Found this on StackOverflow.
            // I forgot the post and user : (.
            // It counts the occurence of one specific value.
            // Thank you kind stranger.
            var userGroup = sd.User.GroupBy(i => i);
            foreach (var g in userGroup)
            {
                countedUser.Add(g.Key + @" : " + g.Count());
            }
            userGroup = null;
        }

        public void MostPopularColor()
        {
            var colorGroup = sd.ColorList.GroupBy(i => i);
            foreach (var c in colorGroup)
            {
                countedColor.Add(c.Key + @" : " + c.Count());
            }
            colorGroup = null;
        }

        public void MostPopularCoord()
        {
            var coordGroup = Coords.GroupBy(i => i);
            foreach (var c in coordGroup)
            {
                CountedCoords.Add(c.Key + @" : " + c.Count());
            }
            coordGroup = null;
        }

        public void ClearList()
        {
            sd.ColorList.Clear();
            Array.Clear(XArray, 0, XArray.Length);
            XArray = null;
            Array.Clear(YArray, 0, YArray.Length);
            YArray = null;
            Array.Clear(Coords, 0, Coords.Length);
            Coords = null;
            sd.User.Clear();
        }

        public void DataInserter()
        {
            using (SqlConnection conn = new(connStr))
            {
                string Temp = new("");
                string[] InserterArray;
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                foreach (string s in File.ReadLines(ReadString))
                {
                    Temp = s;
                    Temp = Temp.Replace("UTC", "");
                    Temp = Temp.Replace('"', ' ');
                    InserterArray = Temp.Split(',');
                    InserterArray[3].Trim(' ');
                    InserterArray[4].Trim(' ');

                    SqlCommand command = new("INSERT INTO RedditPlaces (PixelTime, UserUUID, Color, XCoord, YCoord) " + "VALUES (@pixelTime, @userUUID, @color, @xCoord, @yCoord)", conn, transaction);
                    command.Parameters.Add("@PixelTime", System.Data.SqlDbType.DateTime2).Value = DateTime.Parse(InserterArray[0]);
                    command.Parameters.Add("@UserUUID", System.Data.SqlDbType.NVarChar).Value = InserterArray[1].ToString();
                    command.Parameters.Add("@color", System.Data.SqlDbType.NVarChar).Value = InserterArray[2].ToString();
                    command.Parameters.Add("@xCoord", System.Data.SqlDbType.Int).Value = int.Parse(InserterArray[3]);
                    command.Parameters.Add("@yCoord", System.Data.SqlDbType.Int).Value = int.Parse(InserterArray[4]);
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
                transaction.Dispose();
            }
        }

        ~DataRetriever()
        {

        }
    }
}
