using KartverketRegister.Models;
using KartverketRegister.Auth;
using MySql.Data.MySqlClient;

using System.Data;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Diagnostics;


namespace KartverketRegister.Utils
{
    // SQL queries for alt som har med Marker/obstacler å gjøre. 
    public class SequelSuperAdmin : SequelBase
    {

        public SequelSuperAdmin(string dbIP, string dbname) : base(dbIP, dbname) // calls base constructor
        { }
        public SequelSuperAdmin() : base() 
        { }
        public List<AppUserDto> UserFetcher(string FullName = "")
        {
            List<AppUserDto> Users = new List<AppUserDto>();
            conn.Open();
            string sql = @"SELECT UserId,FirstName,LastName,Organization,Email,UserType,CreatedAt 
                FROM Users 
                WHERE LOWER(CONCAT(FirstName, ' ', LastName)) LIKE @SearchInput
            ;";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@SearchInput", $"%{FullName.ToLower()}%");


                using (var reader = cmd.ExecuteReader())
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
                        Users.Add(User);
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
                AND UserType = @UserType
            ;";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@SearchInput", $"%{FullName.ToLower()}%");
                cmd.Parameters.AddWithValue("@UserType", UserType);


                using (var reader = cmd.ExecuteReader())
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
                        Users.Add(User);
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
                FROM Users 
                WHERE UserId = @UserId
            ;";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", UserId);

                using (var reader = cmd.ExecuteReader())
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
            return User;
        }
        public GeneralResponse SetUserRole(AppUserDto UserData)
        {

            conn.Open();
            GeneralResponse Response = null;
            string sql = @"Update Users 
                Set UserType = @UserType 
                WHERE UserId = @UserId
            ;";
            using (var cmd = new MySqlCommand(sql, conn))
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
                    Response =  new GeneralResponse(false, $"Failed changing role {ex.Message}");
                }

            }
            conn.Close();
            return Response;
        }
        public GeneralResponse DeleteUser(int UserId)
        {
            conn.Open();
            GeneralResponse Response = null;
            string sql = @"DELETE FROM Users  
                WHERE UserId = @UserId
            ;";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                try
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.ExecuteNonQuery();
                    Response = new GeneralResponse(true, "Successfully deleted User");
                }
                catch (Exception ex)
                {
                    Response = new GeneralResponse(false, $"Failed Deleting user {ex.Message}");
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
            catch (Exception e)
            {
                return new GeneralResponse(false, $"Notification failed sending to user {UserId}");
            }
        }
        // NEW SHIT
        public List<Marker> FetchAllUnassignedMarkers()
        {
            conn.Open();
            
            List<Marker> Markers = new List<Marker>();
            string sql = @"
            SELECT rm.*
            FROM RegisteredMarkers rm
            LEFT JOIN ReviewAssign ra
                ON rm.MarkerId = ra.MarkerId
            WHERE ra.MarkerId IS NULL
                AND rm.State = 'Unseen';
            ";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                //cmd.Parameters.AddWithValue("@markerStatus", markerStatus);

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
                        //mrk.UserName = reader["Name"] as string;
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
      
        public List<Marker> FetchAllMarkers(string markerStatus)
        {
            conn.Open();

            List<Marker> Markers = new List<Marker>();
            string sql;
            if (markerStatus == "Everything")
            {
                sql = @"
                    SELECT 
                        rm.*,
                        CONCAT(sub.FirstName, ' ', sub.LastName) AS Name,
                        sub.Email AS SubmitterEmail
                    FROM RegisteredMarkers AS rm
                    LEFT JOIN Users AS sub
                        ON rm.UserId = sub.UserId
                    WHERE rm.State != @markerStatus
                ;";
            } else
            {
                sql = @"
                    SELECT 
                        rm.*,
                        CONCAT(sub.FirstName, ' ', sub.LastName) AS Name,
                        sub.Email AS SubmitterEmail
                    FROM RegisteredMarkers AS rm
                    LEFT JOIN Users AS sub
                        ON rm.UserId = sub.UserId
                    WHERE rm.State = @markerStatus
                ;";
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
                        mrk.UserName = reader["Name"] as string;
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
        public GeneralResponse AssignReview(ReviewAssign RA)
        {
            string sql = @"
                INSERT INTO ReviewAssign (UserId, MarkerId)
                Values (@UserId, @MarkerId)
            ";
            try
            {
                using (var cmd = new MySqlCommand(sql, conn))
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
    }
}
