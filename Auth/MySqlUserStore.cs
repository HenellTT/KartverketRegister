using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KartverketRegister.Auth
{
    public class MySqlUserStore :
        IUserStore<AppUser>,
        IUserPasswordStore<AppUser>,
        IUserRoleStore<AppUser>,
        IUserEmailStore<AppUser>
    {
        private readonly MySqlConnection _conn;

        public MySqlUserStore(MySqlConnection conn)
        {
            _conn = conn;
        }

        #region IUserStore<AppUser>
        public Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken)
        {
            // Hash the password first
            var passwordHasher = new PasswordHasher<AppUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, user.Password);

            string query = @"
                INSERT INTO Users (Name, FirstName, LastName, Organization, UserType, PasswordHash, Email, CreatedAt)
                VALUES (@n, @fn, @ln, @org, @role, @pw, @email, NOW());";

            using var cmd = new MySqlCommand(query, _conn);
            cmd.Parameters.AddWithValue("@n", user.Name);
            cmd.Parameters.AddWithValue("@fn", user.FirstName);
            cmd.Parameters.AddWithValue("@ln", user.LastName);
            cmd.Parameters.AddWithValue("@org", user.Organization);
            cmd.Parameters.AddWithValue("@role", user.UserType ?? "User");
            cmd.Parameters.AddWithValue("@pw", user.PasswordHash);
            cmd.Parameters.AddWithValue("@email", user.Email);

            cmd.ExecuteNonQuery();
            user.Id = (int)cmd.LastInsertedId;

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
        {
            using var cmd = new MySqlCommand("DELETE FROM Users WHERE UserId=@id;", _conn);
            cmd.Parameters.AddWithValue("@id", user.Id);
            cmd.ExecuteNonQuery();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
        {
            // Optional: implement full update logic
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using var cmd = new MySqlCommand("SELECT * FROM Users WHERE UserId=@id;", _conn);
            cmd.Parameters.AddWithValue("@id", userId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return Task.FromResult(MapReaderToUser(reader));
            }
            return Task.FromResult<AppUser>(null);
        }

        public Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using var cmd = new MySqlCommand("SELECT * FROM Users WHERE UPPER(Name)=@n;", _conn);
            cmd.Parameters.AddWithValue("@n", normalizedUserName.ToUpper());
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return Task.FromResult(MapReaderToUser(reader));
            }
            return Task.FromResult<AppUser>(null);
        }

        public Task<string> GetUserIdAsync(AppUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.Id.ToString());

        public Task<string> GetUserNameAsync(AppUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.Name);

        public Task SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken)
        {
            user.Name = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.Name.ToUpper());

        public Task SetNormalizedUserNameAsync(AppUser user, string normalizedName, CancellationToken cancellationToken)
        {
            // optional: no-op
            return Task.CompletedTask;
        }

        public void Dispose() { }
        #endregion

        #region IUserPasswordStore<AppUser>
        public Task SetPasswordHashAsync(AppUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(AppUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(AppUser user, CancellationToken cancellationToken)
            => Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        #endregion

        #region IUserRoleStore<AppUser>
        public Task AddToRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            using var cmd = new MySqlCommand("UPDATE Users SET UserType=@role WHERE UserId=@id;", _conn);
            cmd.Parameters.AddWithValue("@role", roleName);
            cmd.Parameters.AddWithValue("@id", user.Id);
            cmd.ExecuteNonQuery();
            user.UserType = roleName;
            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            using var cmd = new MySqlCommand("UPDATE Users SET UserType='User' WHERE UserId=@id;", _conn);
            cmd.Parameters.AddWithValue("@id", user.Id);
            cmd.ExecuteNonQuery();
            user.UserType = "User";
            return Task.CompletedTask;
        }

        public Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken cancellationToken)
        {
            IList<string> roles = new List<string> { user.UserType ?? "User" };
            return Task.FromResult(roles);
        }

        public Task<bool> IsInRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
            => Task.FromResult(string.Equals(user.UserType, roleName, StringComparison.OrdinalIgnoreCase));

        public Task<IList<AppUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            IList<AppUser> users = new List<AppUser>();
            string query = "SELECT * FROM Users WHERE UserType=@r;";
            using var cmd = new MySqlCommand(query, _conn);
            cmd.Parameters.AddWithValue("@r", roleName);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                users.Add(MapReaderToUser(reader));
            }
            return Task.FromResult(users);
        }
        #endregion

        #region IUserEmailStore<AppUser>
        public Task SetEmailAsync(AppUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(AppUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.Email);

        public Task<bool> GetEmailConfirmedAsync(AppUser user, CancellationToken cancellationToken)
            => Task.FromResult(true); // implement if you have confirmation

        public Task SetEmailConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken)
        {
            // optional, no-op
            return Task.CompletedTask;
        }

        public Task<AppUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            using var cmd = new MySqlCommand("SELECT * FROM Users WHERE UPPER(Email)=@e;", _conn);
            cmd.Parameters.AddWithValue("@e", normalizedEmail.ToUpper());
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return Task.FromResult(MapReaderToUser(reader));
            }
            return Task.FromResult<AppUser>(null);
        }

        public Task<string> GetNormalizedEmailAsync(AppUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.Email?.ToUpper());

        public Task SetNormalizedEmailAsync(AppUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            // optional, no-op
            return Task.CompletedTask;
        }
        #endregion

        #region Helpers
        private AppUser MapReaderToUser(MySqlDataReader reader)
        {
            return new AppUser
            {
                Id = reader.GetInt32("UserId"),
                Name = reader.GetString("Name"),
                LastName = reader.GetString("LastName"),
                FirstName = reader.GetString("FirstName"),
                Organization = reader["Organization"]?.ToString(),
                Email = reader["Email"]?.ToString(),
                UserType = reader["UserType"]?.ToString(),
                PasswordHash = reader["PasswordHash"]?.ToString(),
                CreatedAt = reader.GetDateTime("CreatedAt")
            };
        }
        #endregion
    }
}
