using KartverketRegister.Models;
using KartverketRegister.Models.Other;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection.Metadata;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace KartverketRegister.Utils
{
	// SQL queries for alt som har med Marker/obstacler å gjøre. 
    public class SequelAdmin : SequelBase
    {

        public SequelAdmin(string dbIP, string dbname) : base(dbIP, dbname) // calls base constructor
        { }
        
        public List<Marker> FetchAllMarkers(string markerStatus)
        {
            conn.Open();
            string sql;
            List<Marker> Markers = new List<Marker>();
            if (markerStatus == "Everything")
            {
                sql = "SELECT * FROM RegisteredMarkers WHERE State != @markerStatus";
            } else
            {
                sql = "SELECT * FROM RegisteredMarkers WHERE State = @markerStatus";
            }

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@markerStatus", markerStatus);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Marker mrk = new Marker();

                        mrk.Type = reader["Type"] as string;
                        mrk.Description = reader["Description"] as string;
                        mrk.Lat = Convert.ToDouble(reader["Lat"]);
                        mrk.Lng = Convert.ToDouble(reader["Lng"]);

                        mrk.HeightM = reader["HeightM"] != DBNull.Value ? reader.GetDecimal("HeightM") : (decimal?)null;
                        mrk.HeightMOverSea = reader["HeightMOverSea"] != DBNull.Value ? reader.GetDecimal("HeightMOverSea") : (decimal?)null;
                        mrk.Organization = reader["Organization"] as string;
                        mrk.AccuracyM = reader["AccuracyM"] != DBNull.Value ? reader.GetDecimal("AccuracyM") : (decimal?)null;
                        mrk.ObstacleCategory = reader["ObstacleCategory"] as string;
                        mrk.IsTemporary = reader["IsTemporary"] != DBNull.Value && Convert.ToBoolean(reader["IsTemporary"]);
                        mrk.Lighting = reader["Lighting"] as string;
                        mrk.Source = reader["Source"] as string;
                        mrk.State = reader["State"] as string;

                        mrk.MarkerId = reader["MarkerId"] != DBNull.Value ? Convert.ToInt32(reader["MarkerId"]) : (int?)null;

                        mrk.UserId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : (int?)null;
                        mrk.ReviewedBy = reader["ReviewedBy"] != DBNull.Value ? Convert.ToInt32(reader["ReviewedBy"]) : (int?)null;
                        mrk.ReviewComment = reader["ReviewComment"] != DBNull.Value ? reader["ReviewComment"].ToString() : null;

                        Markers.Add(mrk);
                    }
                }
            }
            Console.WriteLine($"Adm mrk lat1: {Markers[0].Lat}");
            Console.WriteLine($"Adm mrk lng: {Markers[0].Lng}");

            conn.Close();
            return Markers;
        }
        
    }
}
