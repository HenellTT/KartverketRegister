using MySql.Data.MySqlClient;
using KartverketRegister.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace KartverketRegister.Utils
{
    // SQL queries for alt som har med Marker/obstacler å gjøre. 
    public class SequelMarker : SequelBase
    {

        public SequelMarker(string dbIP, string dbname) : base(dbIP, dbname) // calls base constructor
        { }
        public SequelMarker() : base() // calls base constructor
        { }
        public void SaveMarker(
            string type,
            string description,
            double lat,
            double lng,
            int? userId = null,
            string? organization = null,
            string? state = "Unseen",
            decimal? heightM = null,
            decimal? heightMOverSea = null,
            decimal? accuracyM = null,
            string? obstacleCategory = null,
            bool isTemporary = false,
            string? lighting = null,
            int? submittedBy = null,
            int? reviewedBy = null,
            string? reviewComment = null,
            string? source = null,
            string? geojson = null
        )
        {
            conn.Open();

            string sql = @"
                INSERT INTO RegisteredMarkers 
                (Type, Description, Lat, Lng, UserId, Organization, State, HeightM, HeightMOverSea, AccuracyM, 
                 ObstacleCategory, IsTemporary, Lighting, SubmittedBy, ReviewedBy, ReviewComment, LastUpdated, Source, GeoJson)
                VALUES 
                (@Type, @Description, @Lat, @Lng, @UserId, @Organization, @State, @HeightM, @HeightMOverSea, @AccuracyM,
                 @ObstacleCategory, @IsTemporary, @Lighting, @SubmittedBy, @ReviewedBy, @ReviewComment, @LastUpdated, @Source, @GeoJson);
                ";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                // Required fields
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Description", description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Lat", lat);
                cmd.Parameters.AddWithValue("@Lng", lng);
                cmd.Parameters.AddWithValue("@UserId", userId ?? (object)DBNull.Value);

                // Optional fields
                cmd.Parameters.AddWithValue("@Organization", organization ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@State", state ?? "Unseen");
                cmd.Parameters.AddWithValue("@HeightM", heightM ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HeightMOverSea", heightMOverSea ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AccuracyM", accuracyM ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ObstacleCategory", obstacleCategory ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsTemporary", isTemporary);
                cmd.Parameters.AddWithValue("@Lighting", lighting ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SubmittedBy", submittedBy ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ReviewedBy", reviewedBy ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ReviewComment", reviewComment ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LastUpdated", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@Source", source ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@GeoJson", geojson);

                cmd.ExecuteNonQuery();
            }

            conn.Close();
        }
        public List<Marker> FetchMyMarkers(int UserId)
        {
            conn.Open();
            List<Marker> Markers = new List<Marker>();
            string sql = "SELECT * FROM RegisteredMarkers WHERE UserId = @userId";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@userId", UserId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Marker mrk = new Marker();

                        mrk.Type = reader["Type"] as string;
                        mrk.Description = reader["Description"] as string;
                        
                        mrk.Lat = reader.GetDouble("Lat");
                        mrk.Lng = reader.GetDouble("Lng");

                        mrk.HeightM = reader["HeightM"] != DBNull.Value ? reader.GetDecimal("HeightM") : (decimal?)null;
                        mrk.HeightMOverSea = reader["HeightMOverSea"] != DBNull.Value ? reader.GetDecimal("HeightMOverSea") : (decimal?)null;
                        mrk.Organization = reader["Organization"] as string;
                        mrk.AccuracyM = reader["AccuracyM"] != DBNull.Value ? reader.GetDecimal("AccuracyM") : (decimal?)null;
                        mrk.ObstacleCategory = reader["ObstacleCategory"] as string;
                        mrk.IsTemporary = reader["IsTemporary"] != DBNull.Value && Convert.ToBoolean(reader["IsTemporary"]);
                        mrk.Lighting = reader["Lighting"] as string;
                        mrk.Source = reader["Source"] as string;
                        mrk.State = reader["State"] as string;
                        mrk.Date = Convert.ToDateTime(reader["Date"]);
                        mrk.GeoJson = reader["GeoJson"] != DBNull.Value ? (string)reader["GeoJson"] : null;


                        mrk.MarkerId = reader["MarkerId"] != DBNull.Value ? Convert.ToInt32(reader["MarkerId"]) : (int?)null;

                        mrk.UserId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : (int?)null;
                        mrk.ReviewedBy = reader["ReviewedBy"] != DBNull.Value ? Convert.ToInt32(reader["ReviewedBy"]) : (int?)null;
                        mrk.ReviewComment = reader["ReviewComment"] != DBNull.Value ? reader["ReviewComment"].ToString() : null;

                        Markers.Add(mrk);
                    }
                }
            }

            conn.Close();
            return Markers;
        }
        public Marker FetchMarkerById(int markerId)
        {
            Marker mrk = new Marker(); // Will hold the result

            conn.Open();
            string sql = @"
                SELECT 
                    rm.*,
                    u.Name
                FROM RegisteredMarkers rm
                LEFT JOIN Users u ON rm.UserId = u.UserId
                WHERE rm.MarkerId = @markerId
                LIMIT 1;
            ";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@markerId", markerId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) // Only read first row
                    {


                        mrk.Type = reader["Type"] as string;
                        mrk.Description = reader["Description"] as string;
                        mrk.Lat = reader.GetDouble("Lat");
                        mrk.Lng = reader.GetDouble("Lng");

                        mrk.HeightM = reader["HeightM"] != DBNull.Value ? reader.GetDecimal("HeightM") : (decimal?)null;
                        mrk.HeightMOverSea = reader["HeightMOverSea"] != DBNull.Value ? reader.GetDecimal("HeightMOverSea") : (decimal?)null;
                        mrk.Organization = reader["Organization"] as string;
                        mrk.AccuracyM = reader["AccuracyM"] != DBNull.Value ? reader.GetDecimal("AccuracyM") : (decimal?)null;
                        mrk.ObstacleCategory = reader["ObstacleCategory"] as string;
                        mrk.IsTemporary = reader["IsTemporary"] != DBNull.Value && Convert.ToBoolean(reader["IsTemporary"]);
                        mrk.Lighting = reader["Lighting"] as string;
                        mrk.Source = reader["Source"] as string;
                        mrk.State = reader["State"] as string;
                        mrk.GeoJson = reader["GeoJson"] != DBNull.Value ? (string)reader["GeoJson"] : null;

                        mrk.MarkerId = reader["MarkerId"] != DBNull.Value ? Convert.ToInt32(reader["MarkerId"]) : (int?)null;

                        mrk.UserId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : (int?)null;
                        mrk.ReviewedBy = reader["ReviewedBy"] != DBNull.Value ? Convert.ToInt32(reader["ReviewedBy"]) : (int?)null;
                        mrk.ReviewComment = reader["ReviewComment"] != DBNull.Value ? reader["ReviewComment"].ToString() : null;

                    }
                }
            }

            conn.Close();

            // Fallback if marker not found
            if (mrk.MarkerId == null)
            {
                return null;
            }

            return mrk;
        }
        public void DeleteMarkerById(int markerId)
        {
            conn.Open();
            string sql = "DELETE FROM RegisteredMarkers WHERE MarkerId = @MarkerId";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MarkerId", markerId);
                cmd.ExecuteNonQuery();

            }

            conn.Close();
            int UserId = GetUserIdFromMarkerId(markerId);
            Notificator.SendNotification(UserId, $"Your Submission has been Removed", "Warning", markerId);

        }
        public void SetMarkerStatusSeen(int markerId)
        {
            conn.Open();
            string sql = "UPDATE RegisteredMarkers SET Status = 'Seen' WHERE MarkerId = @MarkerId";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MarkerId", markerId);
                cmd.ExecuteNonQuery();

            }

            conn.Close();

        }
        public void ApproveMarker(int markerId, string ReviewComment, int ReviewerId)
        {
            conn.Open();
            string sql = "UPDATE RegisteredMarkers SET State = 'Accepted', ReviewComment = @ReviewComment, ReviewedBy = @UserId WHERE MarkerId = @MarkerId";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MarkerId", markerId);
                cmd.Parameters.AddWithValue("@ReviewComment", ReviewComment);
                cmd.Parameters.AddWithValue("@UserId", ReviewerId);
                cmd.ExecuteNonQuery();

            }
            conn.Close();
            int UserId = GetUserIdFromMarkerId(markerId);
            Notificator.SendNotification(UserId, $"Your Submission has been approved", "Info", markerId);

        }
        public void RejectMarker(int markerId, string ReviewComment, int ReviewerId)
        {
            conn.Open();
            string sql = "UPDATE RegisteredMarkers SET State = 'Rejected', ReviewComment = @ReviewComment, ReviewedBy = @UserId WHERE MarkerId = @MarkerId";


            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MarkerId", markerId);
                cmd.Parameters.AddWithValue("@ReviewComment", ReviewComment);
                cmd.Parameters.AddWithValue("@UserId", ReviewerId);
                cmd.ExecuteNonQuery();

            }

            conn.Close();
            int UserId = GetUserIdFromMarkerId(markerId);
            Notificator.SendNotification(UserId, $"Your Submission has been rejected", "Info", markerId);

        }
        public int GetUserIdFromMarkerId(int MarkerId)
        {
            conn.Open();
            string sql = "SELECT UserId FROM RegisteredMarkers WHERE MarkerId = @MarkerId";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MarkerId", MarkerId);
                using (var Reader = cmd.ExecuteReader())
                {
                    int UserId = 0;
                    if (Reader.Read())
                    {
                        UserId = Reader.GetInt32("UserId");
                    }

                    return UserId;
                }
            }

        }
        public List<LocationModel> GetObstacles() { // add lat lng l8r to limit amount of markers fetched by user;
            conn.Open();

            List<LocationModel> markers = new List<LocationModel>();
            
            string sql = @"
                SELECT Lat,Lng,ObstacleCategory,GeoJson
                FROM RegisteredMarkers
               
            ";


            using (var cmd = new MySqlCommand(sql, conn))
            {
               
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LocationModel LM = new LocationModel();
                        LM.ObstacleCategory = reader["ObstacleCategory"] as string;
                        LM.GeoJson = reader["GeoJson"] as string;
                        LM.Lat = reader.GetDouble("Lat");
                        LM.Lng = reader.GetDouble("Lng");
                        markers.Add(LM);
                    }
                }
            }
            conn.Close();
            return markers;
        }

    }


}

