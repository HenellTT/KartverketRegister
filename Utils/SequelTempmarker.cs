using KartverketRegister.Models;
using MySql.Data.MySqlClient;

namespace KartverketRegister.Utils
{
    // SQL-queries for midlertidige markører (Markers-tabellen)
    public class SequelTempmarker : SequelBase
    {
        public SequelTempmarker(string dbIP, string dbname) : base(dbIP, dbname) { }

        public void SaveMarker(string type, string description, double lat, double lng, decimal height, int UserId, string GeoJson)
        {
            conn.Open();

            string sql = @"INSERT INTO Markers (Type, Description, Lat, Lng, HeightMOverSea, UserId, GeoJson) 
                           VALUES (@type, @description, @lat, @lng, @Height, @UserId, @GeoJson)";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@lat", lat);
                cmd.Parameters.AddWithValue("@lng", lng);
                cmd.Parameters.AddWithValue("@Height", height);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@GeoJson", GeoJson);
                cmd.ExecuteNonQuery();
            }

            conn.Close();
        }

        public List<TempMarker> FetchMyMarkers(int UserId)
        {
            conn.Open();
            List<TempMarker> Markers = new List<TempMarker>();
            string sql = "SELECT * FROM Markers WHERE UserId = @userId";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@userId", UserId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TempMarker mrk = new TempMarker(reader);
                        Markers.Add(mrk.HtmlEncodeStrings());
                    }
                }
            }

            conn.Close();
            return Markers;
        }

        public TempMarker FetchMarkerById(int markerId)
        {
            TempMarker mrk = null;

            conn.Open();
            string sql = "SELECT * FROM Markers WHERE MarkerId = @markerId LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@markerId", markerId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        mrk = new TempMarker(reader);
                    }
                }
            }

            conn.Close();

            // Fallback if marker not found
            if (mrk == null)
            {
                mrk = new TempMarker
                {
                    Description = "Not found",
                    MarkerId = -1,
                    Type = "None",
                    Lat = 0,
                    Lng = 0,
                    UserId = -1
                };
            }

            return mrk.HtmlEncodeStrings();
        }

        public GeneralResponse DeleteMarkerById(int markerId, int UserId)
        {
            try
            {
                conn.Open();
                string sql = "DELETE FROM Markers WHERE MarkerId = @MarkerId AND UserId = @UserId";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MarkerId", markerId);
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
                return new GeneralResponse(true, "Marker Deleted Successfully");
            }
            catch (Exception ex)
            {
                return new GeneralResponse(false, $"Error deleting marker: {ex.Message}");
            }
        }
    }
}
