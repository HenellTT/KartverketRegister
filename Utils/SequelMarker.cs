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
            string? source = null
        )
        {
            conn.Open();

            string sql = @"
                INSERT INTO RegisteredMarkers 
                (Type, Description, Lat, Lng, UserId, Organization, State, HeightM, HeightMOverSea, AccuracyM, 
                 ObstacleCategory, IsTemporary, Lighting, SubmittedBy, ReviewedBy, ReviewComment, LastUpdated, Source)
                VALUES 
                (@Type, @Description, @Lat, @Lng, @UserId, @Organization, @State, @HeightM, @HeightMOverSea, @AccuracyM,
                 @ObstacleCategory, @IsTemporary, @Lighting, @SubmittedBy, @ReviewedBy, @ReviewComment, @LastUpdated, @Source);
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

                cmd.ExecuteNonQuery();
            }
            Console.WriteLine($"laat: {lat}");
            Console.WriteLine($"lnng: {lng}");
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
        public TempMarker FetchMarkerById(int markerId)
        {
            TempMarker mrk = null; // Will hold the result

            conn.Open();
            string sql = "SELECT MarkerId, Lat, Lng, Description, UserId, Type FROM RegisteredMarkers WHERE MarkerId = @markerId LIMIT 1";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@markerId", markerId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) // Only read first row
                    {
                        mrk = new TempMarker
                        {
                            MarkerId = reader.GetInt32("MarkerId"),
                            UserId = reader.IsDBNull("UserId") ? (int?)null : reader.GetInt32("UserId"),
                            Lat = reader.GetDouble("Lat"),
                            Lng = reader.GetDouble("Lng"),
                            Type = reader.IsDBNull("Type") ? null : reader.GetString("Type"),
                            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description")
                        };
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
            string sql = "DELETE FROM RegisteredMarkers WHERE MarkerId = @MarkerId";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MarkerId", markerId);
                cmd.ExecuteNonQuery();

            }

            conn.Close();

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
        public void ApproveMarker(int markerId, string ReviewComment)
        {
            conn.Open();
            string sql = "UPDATE RegisteredMarkers SET Status = 'Accepted', ReviewComment = @ReviewComment WHERE MarkerId = @MarkerId";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MarkerId", markerId);
                cmd.Parameters.AddWithValue("@ReviewComment", ReviewComment);
                cmd.ExecuteNonQuery();

            }

            conn.Close();

        }
        public void RejectMarker(int markerId, string ReviewComment)
        {
            conn.Open();
            string sql = "UPDATE RegisteredMarkers SET Status = 'Rejected', ReviewComment = @ReviewComment WHERE MarkerId = @MarkerId";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MarkerId", markerId);
                cmd.Parameters.AddWithValue("@ReviewComment", ReviewComment);
                cmd.ExecuteNonQuery();

            }

            conn.Close();

        }


    }
}
