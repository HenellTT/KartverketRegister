using KartverketRegister.Models;
using KartverketRegister.Auth;
using MySql.Data.MySqlClient;

namespace KartverketRegister.Utils
{
    // SQL-queries for superadmin - brukeradministrasjon og global markør-oversikt
    public class SequelSuperAdmin : SequelBase
    {
        public SequelSuperAdmin(string dbIP, string dbname) : base(dbIP, dbname) { }
        public SequelSuperAdmin() : base() { }

        public List<AppUserDto> UserFetcher(string FullName = "")
        {
            List<AppUserDto> Users = new List<AppUserDto>();
            conn.Open();
            string sql = @"SELECT UserId,FirstName,LastName,Organization,Email,UserType,CreatedAt 
                FROM Users 
                WHERE LOWER(CONCAT(FirstName, ' ', LastName)) LIKE @SearchInput;";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@SearchInput", $"%{FullName.ToLower()}%");

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AppUserDto User = new AppUserDto
                        {
                            Id = Convert.ToInt32(reader["UserId"]),
                            LastName = reader["LastName"]?.ToString(),
                            FirstName = reader["FirstName"]?.ToString(),
                            Organization = reader["Organization"]?.ToString(),
                            Email = reader["Email"]?.ToString(),
                            UserType = reader["UserType"]?.ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        };
                        Users.Add(User.HtmlEncodeStrings());
                    }
                }
            }
            conn.Close();
            return Users;
        }

        public List<AppUserDto> AdvUserFetcher(string UserType, string FullName = "")
        {
            List<AppUserDto> Users = new List<AppUserDto>();
            conn.Open();
            string sql = @"SELECT UserId,FirstName,LastName,Organization,Email,UserType,CreatedAt 
                FROM Users 
                WHERE LOWER(CONCAT(FirstName, ' ', LastName)) LIKE @SearchInput
                AND UserType = @UserType;";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@SearchInput", $"%{FullName.ToLower()}%");
                cmd.Parameters.AddWithValue("@UserType", UserType);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AppUserDto User = new AppUserDto
                        {
                            Id = Convert.ToInt32(reader["UserId"]),
                            LastName = reader["LastName"]?.ToString(),
                            FirstName = reader["FirstName"]?.ToString(),
                            Organization = reader["Organization"]?.ToString(),
                            Email = reader["Email"]?.ToString(),
                            UserType = reader["UserType"]?.ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        };
                        Users.Add(User.HtmlEncodeStrings());
                    }
                }
            }
            conn.Close();
            return Users;
        }

        public AppUserDto FetchUser(int UserId)
        {
            AppUserDto User = new AppUserDto();
            conn.Open();
            string sql = @"SELECT UserId,FirstName,LastName,Organization,Email,UserType,CreatedAt 
                FROM Users WHERE UserId = @UserId;";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", UserId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        User = new AppUserDto
                        {
                            Id = Convert.ToInt32(reader["UserId"]),
                            LastName = reader["LastName"]?.ToString(),
                            FirstName = reader["FirstName"]?.ToString(),
                            Organization = reader["Organization"]?.ToString(),
                            Email = reader["Email"]?.ToString(),
                            UserType = reader["UserType"]?.ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        };
                    }
                }
            }
            conn.Close();
            return User.HtmlEncodeStrings();
        }

        public GeneralResponse SetUserRole(AppUserDto UserData)
        {
            conn.Open();
            GeneralResponse Response;
            string sql = "UPDATE Users SET UserType = @UserType WHERE UserId = @UserId;";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                try
                {
                    cmd.Parameters.AddWithValue("@UserId", UserData.Id);
                    cmd.Parameters.AddWithValue("@UserType", UserData.UserType);
                    cmd.ExecuteNonQuery();
                    Notificator.SendNotification(UserData.Id, $"Your Role in the system has been changed to {UserData.UserType}", "Info");
                    Response = new GeneralResponse(true, "Successfully changed role");
                }
                catch (Exception ex)
                {
                    Response = new GeneralResponse(false, $"Failed changing role: {ex.Message}");
                }
            }
            conn.Close();
            return Response;
        }

        public GeneralResponse DeleteUser(int UserId)
        {
            conn.Open();
            GeneralResponse Response;
            string sql = "DELETE FROM Users WHERE UserId = @UserId;";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                try
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.ExecuteNonQuery();
                    Response = new GeneralResponse(true, "Successfully deleted User");
                }
                catch (Exception ex)
                {
                    Response = new GeneralResponse(false, $"Failed deleting user: {ex.Message}");
                }
            }
            conn.Close();
            return Response;
        }

        public GeneralResponse SendNotification(int UserId, string Msg)
        {
            try
            {
                Notificator.SendNotification(UserId, Msg, "Info");
                return new GeneralResponse(true, $"Notification sent to user {UserId}");
            }
            catch
            {
                return new GeneralResponse(false, $"Notification failed sending to user {UserId}");
            }
        }

        // Henter markører som ikke er tildelt en saksbehandler
        public List<Marker> FetchAllUnassignedMarkers()
        {
            conn.Open();
            List<Marker> Markers = new List<Marker>();
            string sql = @"
                SELECT rm.*
                FROM RegisteredMarkers rm
                LEFT JOIN ReviewAssign ra ON rm.MarkerId = ra.MarkerId
                WHERE ra.MarkerId IS NULL AND rm.State = 'Unseen';";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
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
                            MarkerId = reader["MarkerId"] != DBNull.Value ? Convert.ToInt32(reader["MarkerId"]) : null,
                            UserId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : null,
                            ReviewedBy = reader["ReviewedBy"] != DBNull.Value ? Convert.ToInt32(reader["ReviewedBy"]) : null,
                            ReviewComment = reader["ReviewComment"] != DBNull.Value ? reader["ReviewComment"].ToString() : null,
                            GeoJson = reader["GeoJson"] != DBNull.Value ? (string)reader["GeoJson"] : null
                        };
                        Markers.Add(mrk.HtmlEncodeStrings());
                    }
                }
            }

            conn.Close();
            return Markers;
        }

        public List<Marker> FetchAllMarkers(string markerStatus)
        {
            conn.Open();
            List<Marker> Markers = new List<Marker>();

            string sql = markerStatus == "Everything"
                ? @"SELECT rm.*, CONCAT(sub.FirstName, ' ', sub.LastName) AS Name, sub.Email AS SubmitterEmail
                    FROM RegisteredMarkers AS rm
                    LEFT JOIN Users AS sub ON rm.UserId = sub.UserId
                    WHERE rm.State != @markerStatus;"
                : @"SELECT rm.*, CONCAT(sub.FirstName, ' ', sub.LastName) AS Name, sub.Email AS SubmitterEmail
                    FROM RegisteredMarkers AS rm
                    LEFT JOIN Users AS sub ON rm.UserId = sub.UserId
                    WHERE rm.State = @markerStatus;";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@markerStatus", markerStatus);

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
                            Date = Convert.ToDateTime(reader["Date"]),
                            MarkerId = reader["MarkerId"] != DBNull.Value ? Convert.ToInt32(reader["MarkerId"]) : null,
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

        public GeneralResponse AssignReview(ReviewAssign RA)
        {
            string sql = "INSERT INTO ReviewAssign (UserId, MarkerId) VALUES (@UserId, @MarkerId)";
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("UserId", RA.UserId);
                    cmd.Parameters.AddWithValue("MarkerId", RA.MarkerId);
                    cmd.ExecuteNonQuery();
                }
                return new GeneralResponse(true, "Assigned to review");
            }
            catch (Exception ex)
            {
                return new GeneralResponse(false, $"Failed: {ex.Message}");
            }
        }

        public GeneralResponse UnAssignAll()
        {
            Open();
            string sql = "DELETE FROM ReviewAssign;";
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                return new GeneralResponse(true, "All assignments removed");
            }
            catch (Exception ex)
            {
                return new GeneralResponse(false, $"Failed: {ex.Message}");
            }
            finally
            {
                Close();
            }
        }
    }
}
