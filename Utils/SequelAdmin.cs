using KartverketRegister.Models;
using MySql.Data.MySqlClient;

namespace KartverketRegister.Utils
{
    // SQL-queries for saksbehandler - henter markører tildelt en spesifikk ansatt
    public class SequelAdmin : SequelBase
    {
        public SequelAdmin(string dbIP, string dbname) : base(dbIP, dbname) { }

        public List<Marker> FetchAllMarkers(string markerStatus, int UserId)
        {
            conn.Open();
            List<Marker> Markers = new List<Marker>();

            string sql = markerStatus == "Everything"
                ? @"SELECT rm.*, CONCAT(sub.FirstName, ' ', sub.LastName) AS Name, sub.Email AS SubmitterEmail
                    FROM ReviewAssign AS ra
                    JOIN Users AS u ON ra.UserId = u.UserId
                    JOIN RegisteredMarkers AS rm ON ra.MarkerId = rm.MarkerId
                    JOIN Users AS sub ON rm.UserId = sub.UserId
                    WHERE u.UserId = @UserId AND rm.State != @markerStatus;"
                : @"SELECT rm.*, CONCAT(sub.FirstName, ' ', sub.LastName) AS Name, sub.Email AS SubmitterEmail
                    FROM ReviewAssign AS ra
                    JOIN Users AS u ON ra.UserId = u.UserId
                    JOIN RegisteredMarkers AS rm ON ra.MarkerId = rm.MarkerId
                    JOIN Users AS sub ON rm.UserId = sub.UserId
                    WHERE u.UserId = @UserId AND rm.State = @markerStatus;";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@markerStatus", markerStatus);
                cmd.Parameters.AddWithValue("@UserId", UserId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Marker mrk = new Marker
                        {
                            Type = reader["Type"] as string,
                            Description = reader["Description"] as string,
                            Lat = Convert.ToDouble(reader["Lat"]),
                            Lng = Convert.ToDouble(reader["Lng"]),
                            HeightM = reader["HeightM"] != DBNull.Value ? reader.GetDecimal("HeightM") : null,
                            HeightMOverSea = reader["HeightMOverSea"] != DBNull.Value ? reader.GetDecimal("HeightMOverSea") : null,
                            Organization = reader["Organization"] as string,
                            AccuracyM = reader["AccuracyM"] != DBNull.Value ? reader.GetDecimal("AccuracyM") : null,
                            ObstacleCategory = reader["ObstacleCategory"] as string,
                            IsTemporary = reader["IsTemporary"] != DBNull.Value && Convert.ToBoolean(reader["IsTemporary"]),
                            Lighting = reader["Lighting"] as string,
                            Source = reader["Source"] as string,
                            State = reader["State"] as string,
                            UserName = reader["Name"] as string,
                            MarkerId = reader["MarkerId"] != DBNull.Value ? Convert.ToInt32(reader["MarkerId"]) : null,
                            Date = Convert.ToDateTime(reader["Date"]),
                            GeoJson = reader["GeoJson"] != DBNull.Value ? (string)reader["GeoJson"] : null,
                            UserId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : null,
                            ReviewedBy = reader["ReviewedBy"] != DBNull.Value ? Convert.ToInt32(reader["ReviewedBy"]) : null,
                            ReviewComment = reader["ReviewComment"] != DBNull.Value ? reader["ReviewComment"].ToString() : null
                        };
                        Markers.Add(mrk.HtmlEncodeStrings());
                    }
                }
            }

            conn.Close();
            return Markers;
        }
    }
}
