using MySql.Data.MySqlClient;
using KartverketRegister.Models;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace KartverketRegister.Utils
{
	// SQL queries for alt som har med tempmarker å gjøre. 
    public class SequelTempmarker: SequelBase
    {
        public SequelTempmarker(string dbIP, string dbname) : base(dbIP, dbname) // calls base constructor
        { }
        public void SaveMarker(string type, string description, double lat, double lng, decimal height)
        {
            conn.Open();

            string sql = "INSERT INTO Markers (Type, Description, Lat, Lng, HeightMOverSea, UserId) " +
                     "VALUES (@type, @description, @lat, @lng, @Height, @UserId)";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                // Parameters protect against SQL injection 
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@lat", lat);
                cmd.Parameters.AddWithValue("@lng", lng);
                cmd.Parameters.AddWithValue("@Height", height);
                cmd.Parameters.AddWithValue("@UserId", 1);
                Console.WriteLine(lat);
                Console.WriteLine(lng);
               
                cmd.ExecuteNonQuery(); // <-- this runs the INSERT
                
            }
            conn.Close();
        }
        public List<TempMarker> FetchMyMarkers(int UserId)
        {
            conn.Open();
            List<TempMarker> Markers = new List<TempMarker>();
            string sql = "SELECT MarkerId,Lat,Lng,Description,UserId,Type,HeightMOverSea FROM Markers WHERE UserId = @userId";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@userId", UserId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TempMarker mrk = new TempMarker();
                        mrk.MarkerId = reader.GetInt32("MarkerId");
                        mrk.UserId = reader.GetInt32("UserId");
                        mrk.Lat = reader.GetDouble("Lat");
                        mrk.Lng = reader.GetDouble("Lng");
                        mrk.Type = reader.GetString("Type");
                        mrk.Description = reader.GetString("Description");
                        mrk.HeightMOverSea = reader.GetDecimal("HeightMOverSea");

                        Markers.Add(mrk);
                    }
                }
            }

            conn.Close();
            return Markers;
        }
        public TempMarker FetchMarkerById(int markerId)
        {
            TempMarker mrk = null; // Will hold the result

            conn.Open();
            string sql = "SELECT MarkerId, Lat, Lng, Description, UserId, HeightMOverSea, Type FROM Markers WHERE MarkerId = @markerId LIMIT 1";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@markerId", markerId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) // Only read first row
                    {
                        mrk = new TempMarker();
                        mrk.MarkerId = reader.GetInt32("MarkerId");
                        mrk.UserId = reader.GetInt32("UserId");
                        mrk.Lat = reader.GetDouble("Lat");
                        mrk.Lng = reader.GetDouble("Lng");
                        mrk.Type = reader.GetString("Type");
                        mrk.Description = reader.GetString("Description");
                        mrk.HeightMOverSea = reader.GetDecimal("HeightMOverSea");

                    }
                }
            }

            conn.Close();

            // Fallback if marker not found
            if (mrk == null)
            {
                mrk = new TempMarker();
                mrk.Description = "No";
                mrk.MarkerId = -1;
                mrk.Type = "No";
                mrk.Lat = 0;
                mrk.Lng = 0;
                mrk.UserId = -1;
            }

            return mrk;
        }
        public void DeleteMarkerById(int markerId)
        {
            conn.Open();
            string sql = "DELETE FROM Markers WHERE MarkerId = @MarkerId";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MarkerId", markerId);
                cmd.ExecuteNonQuery();

            }

            conn.Close();

        }
    }
}