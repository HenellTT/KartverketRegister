using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KartverketRegister.Auth
{
    // We’re faking a full role store — but roles are baked into UserType
    public class MySqlRoleStore : IRoleStore<IdentityRole<int>>
    {
        public Task<IdentityResult> CreateAsync(IdentityRole<int> role, CancellationToken cancellationToken)
            => Task.FromResult(IdentityResult.Success);

        public Task<IdentityResult> DeleteAsync(IdentityRole<int> role, CancellationToken cancellationToken)
            => Task.FromResult(IdentityResult.Success);

        public void Dispose() { } //ingen ressurser

        public Task<IdentityRole<int>> FindByIdAsync(string roleId, CancellationToken cancellationToken)
            => Task.FromResult(new IdentityRole<int> { Id = int.Parse(roleId), Name = roleId });

        public Task<IdentityRole<int>> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
            => Task.FromResult(new IdentityRole<int> { Name = normalizedRoleName });

        public Task<string> GetNormalizedRoleNameAsync(IdentityRole<int> role, CancellationToken cancellationToken)
            => Task.FromResult(role.Name?.ToUpper());

        public Task<string> GetRoleIdAsync(IdentityRole<int> role, CancellationToken cancellationToken)
            => Task.FromResult(role.Id.ToString());

        public Task<string> GetRoleNameAsync(IdentityRole<int> role, CancellationToken cancellationToken)
            => Task.FromResult(role.Name);

        public Task SetNormalizedRoleNameAsync(IdentityRole<int> role, string normalizedName, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task SetRoleNameAsync(IdentityRole<int> role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(IdentityRole<int> role, CancellationToken cancellationToken)
            => Task.FromResult(IdentityResult.Success);
    }
}