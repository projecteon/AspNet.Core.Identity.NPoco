namespace AspNet.Core.Identity.NPoco.Stores {
    using System;
    using System.Collections.Generic;
    using AspNet.Core.Identity.NPoco.Models;
    using Microsoft.AspNetCore.Identity;
    using System.Threading.Tasks;
    using System.Threading;
    using System.Security.Claims;
    using System.Linq;
    using global::NPoco;

    public class RoleStore<TRole> : IRoleClaimStore<TRole>
        where TRole : IdentityRole {
        
        public RoleStore(DatabaseFactory dbFactory) {
            DatabaseFactory = dbFactory;
        }

        DatabaseFactory DatabaseFactory { get; }

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            using (var db = DatabaseFactory.GetDatabase())
                await db.InsertAsync(role);

            return IdentityResult.Success;
        }

        public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            // Update concurrency stamp for optimistic concurrency
            var originalStamp = role.ConcurrencyStamp;
            role.ConcurrencyStamp = Guid.NewGuid().ToString();

            var updateCount = 0;

            using (var db = DatabaseFactory.GetDatabase())
                updateCount = db.UpdateWhere(role, "Id = @0 AND ConcurrencyStamp = @1", role.Id, originalStamp);

            // If no updates, most likely a concurrency issue
            if (updateCount == 0)
                throw new IdentityConcurrencyException();

            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            using (var db = DatabaseFactory.GetDatabase())
                await db.DeleteAsync(role);

            return IdentityResult.Success;
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentOutOfRangeException(nameof(roleName));

            role.Name = roleName;

            return Task.FromResult(0);
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (string.IsNullOrWhiteSpace(normalizedName))
                throw new ArgumentOutOfRangeException(nameof(normalizedName));

            role.NormalizedName = normalizedName;

            return Task.FromResult(0);
        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(roleId))
                throw new ArgumentOutOfRangeException(nameof(roleId));

            using (var db = DatabaseFactory.GetDatabase()) {
                var result = db.FirstOrDefault<TRole>("WHERE Id = @0", roleId);
                return Task.FromResult(result);
            }
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentOutOfRangeException(nameof(normalizedRoleName));

            using (var db = DatabaseFactory.GetDatabase()) {
                var result = db.FirstOrDefault<TRole>("WHERE NormalizedName = @0", normalizedRoleName);
                return Task.FromResult(result);
            }
        }

        public async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = new CancellationToken()) {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            IList<RoleClaim> claims;

            using (var db = DatabaseFactory.GetDatabase())
                claims = await db.FetchAsync<RoleClaim>("WHERE RoleId = @0", role.Id);

            cancellationToken.ThrowIfCancellationRequested();

            return claims.Select(c => c.ToClaim()).ToArray();
        }

        public async Task AddClaimAsync(TRole role, Claim claim,
            CancellationToken cancellationToken = new CancellationToken()) {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            var dbClaim = new RoleClaim(claim, role.Id);

            using (var db = DatabaseFactory.GetDatabase())
                await db.InsertAsync(dbClaim);
        }

        public Task RemoveClaimAsync(TRole role, Claim claim,
            CancellationToken cancellationToken = new CancellationToken()) {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            using (var db = DatabaseFactory.GetDatabase()) {
                db.DeleteWhere<RoleClaim>("RoleId = @0 AND ClaimType = @1 AND ClaimValue = @2",
                    role.Id, claim.Type, claim.Value);
            }

            return Task.FromResult(0);
        }

        #region Disposal

        ~RoleStore() {
            Dispose(false);
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing) {
            // Unused
        }

        #endregion
    }
}
