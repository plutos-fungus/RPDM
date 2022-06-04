using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections.ObjectModel;
using System.IO;

namespace RedditPlaceCanvas.Model
{
    public struct SQLData
    {
        public List<String> ColorList = new();
        //public List<int> XCoord = new();
        //public List<int> YCoord = new();
        //public List<string> Coords = new();
        public List<string> User = new();

        public int j = 0;
        public int[]? XArray;
        public int[]? YArray;
        public string[]? Coords;
    }

    public class DataRetriever
    {
        SQLData sd = new();
        private static readonly string connStr = "";
        //public List<String> ColorList = new();
        //public List<int> XCoord = new();
        //public List<int> YCoord = new();
        //public List<string> Coords = new();
        //public List<string> User = new();

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
                    //while (reader.Read())
                    if (reader.Read())
                    {
                        for (int i = 0; i < 1; i++)
                        {
                            sd.j = Convert.ToInt32(reader["Id"]);
                        }
                    }
                }
            }
            sd.XArray = new int[sd.j];
            sd.YArray = new int[sd.j];
            sd.Coords = new string[sd.j];
        }
        int test = 0;
        public void DataReader()
        {
            using (SqlConnection conn = new(connStr))
            {
                conn.Open();
                SqlCommand command = new("SELECT Id, UserUUID, Color, XCoord, YCoord FROM RedditPlaces ORDER BY Id DESC;", conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    //if (reader.Read())
                    //{
                    //    for (int i = 0; i < sd.j; i++)
                    //    {
                    //        sd.XArray[i] = Convert.ToInt32(reader["XCoord"]);
                    //        sd.YArray[i] = Convert.ToInt32(reader["YCoord"]);
                    //        sd.Coords[i] = Convert.ToString(reader["XCoord"]) + " " + Convert.ToString(reader["YCoord"]);
                    //        sd.ColorList.Add(Convert.ToString(reader["Color"]));
                    //        sd.User.Add(Convert.ToString(reader["UserUUID"]));
                    //    }
                    //}
                    while (reader.Read())
                    {
                        sd.XArray[test] = Convert.ToInt32(reader["XCoord"]);
                        sd.YArray[test] = Convert.ToInt32(reader["YCoord"]);
                        sd.Coords[test] = Convert.ToString(reader["XCoord"]) + " " + Convert.ToString(reader["YCoord"]);
                        sd.ColorList.Add(Convert.ToString(reader["Color"]));
                        sd.User.Add(Convert.ToString(reader["UserUUID"]));
                        test++;
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
        }

        public void MostPopularColor()
        {
            var colorGroup = sd.ColorList.GroupBy(i => i);
            foreach (var c in colorGroup)
            {
                countedColor.Add(c.Key + @" : " + c.Count());
            }
        }

        public void MostPopularCoord()
        {
            var coordGroup = sd.Coords.GroupBy(i => i);
            foreach (var c in coordGroup)
            {
                CountedCoords.Add(c.Key + @" : " + c.Count());
            }
        }

        public void ClearList()
        {
            sd.ColorList.Clear();
            Array.Clear(sd.XArray, 0, sd.XArray.Length);
            sd.XArray = new int[0];
            Array.Clear(sd.YArray, 0, sd.YArray.Length);
            sd.YArray = new int[0];
            Array.Clear(sd.Coords, 0, sd.Coords.Length);
            sd.Coords = new string[0];
            sd.User.Clear();
        }

        ~DataRetriever()
        {

        }
    }
}
