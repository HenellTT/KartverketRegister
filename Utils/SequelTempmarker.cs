using KartverketRegister.Models;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
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
        public void SaveMarker(string type, string description, double lat, double lng, decimal height, int UserId, string GeoJson)
        {
            conn.Open();

            string sql = "INSERT INTO Markers (Type, Description, Lat, Lng, HeightMOverSea, UserId, GeoJson) " +
                     "VALUES (@type, @description, @lat, @lng, @Height, @UserId, @GeoJson)";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                // Parameters protect against SQL injection 
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@lat", lat);
                cmd.Parameters.AddWithValue("@lng", lng);
                cmd.Parameters.AddWithValue("@Height", height);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@GeoJson", GeoJson);
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
            string sql = "SELECT MarkerId,Lat,Lng,Description,UserId,Type,HeightMOverSea,GeoJson FROM Markers WHERE UserId = @userId";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@userId", UserId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TempMarker mrk = new TempMarker(reader);
                        

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
                        mrk = new TempMarker(reader);
                        
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
        public GeneralResponse DeleteMarkerById(int markerId, int UserId)
        {
            try
            {

            conn.Open();
            //string sql = "DELETE FROM Markers WHERE MarkerId = @MarkerId";
            string sql = "DELETE FROM Markers WHERE MarkerId = @MarkerId AND UserId = @UserId";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MarkerId", markerId);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.ExecuteNonQuery();

            }

            conn.Close();
                return new GeneralResponse(true, "Marker Deleted Successfully");

            } catch (Exception ex)
            {
                return new GeneralResponse(false, $"Error deleting marker: {ex}");
            }

        }
    }
}