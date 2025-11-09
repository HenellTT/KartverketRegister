using KartverketRegister.Models;
using KartverketRegister.Auth;
using MySql.Data.MySqlClient;

using System.Data;
using Microsoft.AspNetCore.Mvc;


namespace KartverketRegister.Utils
{
    // SQL queries for alt som har med Marker/obstacler å gjøre. 
    public class SequelSuperAdmin : SequelBase
    {

        public SequelSuperAdmin(string dbIP, string dbname) : base(dbIP, dbname) // calls base constructor
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
    }
}
