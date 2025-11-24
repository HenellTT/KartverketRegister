using KartverketRegister.Models;
using Microsoft.AspNetCore.Components.Routing;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
namespace KartverketRegister.Utils
{
    public static class Notificator
    {
        private static string _connString = $"Server={Constants.DataBaseIp};Port={Constants.DataBasePort};Database={Constants.DataBaseName};User ID=root;Password={Constants.DataBaseRootPassword};";

        public static void SendNotification(int ToUser, string Message, string Type)
        {
            string sqlQuery = @"
                INSERT INTO Notifications 
                (UserId, Message, Type) 
                VALUES 
                (@UserId, @Message, @Type);
            ";
            using (MySqlConnection conn = new MySqlConnection(_connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sqlQuery, conn)) {
                    cmd.Parameters.AddWithValue("@UserId", ToUser);
                    cmd.Parameters.AddWithValue("@Message", Message);
                    cmd.Parameters.AddWithValue("@Type", Type);

                    cmd.ExecuteNonQuery();
                }

            }
        }
        public static void SendNotification(int ToUser, string Message, string Type, int MarkerId)
        {
            string sqlQuery = @"
                INSERT INTO Notifications 
                (UserId, Message, Type, MarkerId) 
                VALUES 
                (@UserId, @Message, @Type, @MarkerId);
            ";
            using (MySqlConnection conn = new MySqlConnection(_connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", ToUser);
                    cmd.Parameters.AddWithValue("@Message", Message);
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@MarkerId", MarkerId);
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public static void SetToRead(int NotificationId, int UserId)
        {
            string sqlQuery = @"
                UPDATE Notifications 
                SET IsRead = 1
                WHERE NotificationId = @NotificationId
                AND UserId = @UserId;
            ";
            using (MySqlConnection conn = new MySqlConnection(_connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@NotificationId", NotificationId);
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public static void DeleteNotificationByUser(int UserId, int NotificationId)
        {
            string sqlQuery = @"
                DELETE FROM Notifications 
                WHERE UserId = @UserId
                AND NotificationId = @NotificationId;
            ";
            using (MySqlConnection conn = new MySqlConnection(_connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@NotificationId", NotificationId);
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public static List<NotificationModel> GetNotificationsByUserId(int UserId)
        {
            string sqlQuery = @"
                SELECT * FROM Notifications 
                WHERE UserId = @UserId;
            ";
            List<NotificationModel> NotificationList = new List<NotificationModel>();
            using (MySqlConnection conn = new MySqlConnection(_connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            NotificationModel Notification = new NotificationModel();
                            Notification.Message = reader["Message"].ToString();
                            Notification.IsRead = reader.GetBoolean("IsRead");
                            Notification.Date = reader.GetDateTime("Date");
                            Notification.Id = reader.GetInt32("NotificationId");

                            Notification.MarkerId = int.TryParse(reader["MarkerId"]?.ToString(), out int mid) ? mid : 0;

                            NotificationList.Add(Notification.HtmlEncodeStrings());
                        }
                    }
                }
            }
            return NotificationList;
        }
    }


}
