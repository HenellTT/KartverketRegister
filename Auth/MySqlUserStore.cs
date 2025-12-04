using KartverketRegister.Utils;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace KartverketRegister.Auth
{
    //brukerlagrin mot MYsql database
    public class MySqlUserStore :
        IUserStore<AppUser>,
        IUserPasswordStore<AppUser>,
        IUserRoleStore<AppUser>,
        IUserEmailStore<AppUser>
    {
        private readonly string _connString;

        public MySqlUserStore(string connString)
        {
            _connString = connString ?? throw new ArgumentNullException(nameof(connString));
        }

        #region IUserStore<AppUser>
        //opprett, slett, oppdater og finn brukere
        public async Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken)
        {
            // Hash password
            var hasher = new PasswordHasher<AppUser>();
            user.PasswordHash = hasher.HashPassword(user, user.Password);

            const string query = @"
                INSERT INTO Users (Name, FirstName, LastName, Organization, UserType, PasswordHash, Email, CreatedAt)
                VALUES (@n, @fn, @ln, @org, @role, @pw, @email, NOW());
            ";

            await using var conn = new MySqlConnection(_connString);
            await conn.OpenAsync(cancellationToken);
            await using var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@n", user.Name);
            cmd.Parameters.AddWithValue("@fn", user.FirstName);
            cmd.Parameters.AddWithValue("@ln", user.LastName);
            cmd.Parameters.AddWithValue("@org", user.Organization);
            cmd.Parameters.AddWithValue("@role", user.UserType ?? "User");
            cmd.Parameters.AddWithValue("@pw", user.PasswordHash);
            cmd.Parameters.AddWithValue("@email", user.Email);

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            user.Id = (int)cmd.LastInsertedId;

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
        {
            const string query = "DELETE FROM Users WHERE UserId=@id;";

            await using var conn = new MySqlConnection(_connString);
            await conn.OpenAsync(cancellationToken);
            await using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", user.Id);

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
        {
            // Optional: implement full update logic
            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<AppUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            const string query = "SELECT * FROM Users WHERE UserId=@id;";

            await using var conn = new MySqlConnection(_connString);
            await conn.OpenAsync(cancellationToken);
            await using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", userId);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return MapReaderToUser(reader);
            }

            return null;
        }

        public async Task<AppUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            const string query = "SELECT * FROM Users WHERE UPPER(Name)=@n;";

            await using var conn = new MySqlConnection(_connString);
            await conn.OpenAsync(cancellationToken);
            await using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@n", normalizedUserName.ToUpper());

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return MapReaderToUser(reader);
            }

            return null;
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
            => Task.CompletedTask;

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
        //legg til, fjern og hent roller for brukere
        public async Task AddToRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            const string query = "UPDATE Users SET UserType=@role WHERE UserId=@id;";

            await using var conn = new MySqlConnection(_connString);
            await conn.OpenAsync(cancellationToken);
            await using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@role", roleName);
            cmd.Parameters.AddWithValue("@id", user.Id);

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            user.UserType = roleName;
        }

        public async Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            const string query = "UPDATE Users SET UserType='User' WHERE UserId=@id;";

            await using var conn = new MySqlConnection(_connString);
            await conn.OpenAsync(cancellationToken);
            await using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", user.Id);

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            user.UserType = "User";
        }

        public Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken cancellationToken)
        {
            IList<string> roles = new List<string> { user.UserType ?? "User" };
            return Task.FromResult(roles);
        }

        public Task<bool> IsInRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
            => Task.FromResult(string.Equals(user.UserType, roleName, StringComparison.OrdinalIgnoreCase));

        public async Task<IList<AppUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            const string query = "SELECT * FROM Users WHERE UserType=@r;";
            IList<AppUser> users = new List<AppUser>();

            await using var conn = new MySqlConnection(_connString);
            await conn.OpenAsync(cancellationToken);
            await using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@r", roleName);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                users.Add(MapReaderToUser(reader));
            }

            return users;
        }

        #endregion

        #region IUserEmailStore<AppUser>
        //sett, hent og finn brukere basert på epost
        public Task SetEmailAsync(AppUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(AppUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.Email);

        public Task<bool> GetEmailConfirmedAsync(AppUser user, CancellationToken cancellationToken)
            => Task.FromResult(true); // optional: implement email confirmation

        public Task SetEmailConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public async Task<AppUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            const string query = "SELECT * FROM Users WHERE UPPER(Email)=@e;";

            await using var conn = new MySqlConnection(_connString);
            await conn.OpenAsync(cancellationToken);
            await using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@e", normalizedEmail.ToUpper());

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
                return MapReaderToUser(reader);

            return null;
        }

        public Task<string> GetNormalizedEmailAsync(AppUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.Email?.ToUpper());

        public Task SetNormalizedEmailAsync(AppUser user, string normalizedEmail, CancellationToken cancellationToken)
            => Task.CompletedTask;

        #endregion

        #region Helpers
        //mappe database leser til AppUser objekt
        private AppUser MapReaderToUser(DbDataReader reader)
        {
            return new AppUser
            {
                Id = Convert.ToInt32(reader["UserId"]),
                Name = reader["Name"]?.ToString(),
                LastName = reader["LastName"]?.ToString(),
                FirstName = reader["FirstName"]?.ToString(),
                Organization = reader["Organization"]?.ToString(),
                Email = reader["Email"]?.ToString(),
                UserType = reader["UserType"]?.ToString(),
                PasswordHash = reader["PasswordHash"]?.ToString(),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
            };
        }

        #endregion
    }
}