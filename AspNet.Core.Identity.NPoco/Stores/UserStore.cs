namespace AspNet.Core.Identity.NPoco.Stores {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using AspNet.Core.Identity.NPoco.Models;
    using global::NPoco;
    using Microsoft.AspNetCore.Identity;

    public class UserStore<TUser, TRole, TUserClaim, TUserLogin, TUserRole> :
        IUserStore<TUser>,
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        IUserTwoFactorRecoveryCodeStore<TUser>,
        IUserTwoFactorStore<TUser>
        where TUser : IdentityUser
        where TRole : IdentityRole
        where TUserClaim : UserClaim
        where TUserLogin : UserLogin
        where TUserRole : UserRole {
        static string UserTableName => typeof(TUser).Name;

        static string RoleTableName => typeof(TRole).Name;

        static string UserClaimTableName => typeof(TUserClaim).Name;

        static string UserLoginTableName => typeof(TUserLogin).Name;

        static string UserRoleTableName => typeof(TUserRole).Name;

        public UserStore(DatabaseFactory dbFactory) {
            DatabaseFactory = dbFactory;
        }

        DatabaseFactory DatabaseFactory { get; }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.UserName = userName;

            return Task.FromResult(0);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.NormalizedUserName = normalizedName;

            return Task.FromResult(0);
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            using (var db = DatabaseFactory.GetDatabase())
                await db.InsertAsync(user);

            return IdentityResult.Success;
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Update concurrency stamp for future use
            var originalStamp = user.ConcurrencyStamp;
            user.ConcurrencyStamp = Guid.NewGuid().ToString();

            int updateCount = 0;

            using (var db = DatabaseFactory.GetDatabase())
                updateCount = db.UpdateWhere(user, "Id = @0 AND ConcurrencyStamp = @1", user.Id, originalStamp);

            // Ensure user was updated, otherwise we had a concurrency issue
            if (updateCount == 0)
                throw new IdentityConcurrencyException();

            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            using (var db = DatabaseFactory.GetDatabase())
                await db.DeleteAsync(user);

            return IdentityResult.Success;
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentOutOfRangeException(nameof(userId));

            using (var db = DatabaseFactory.GetDatabase())
                return Task.FromResult(db.FirstOrDefault<TUser>("WHERE id = @0", userId));
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(normalizedUserName))
                throw new ArgumentOutOfRangeException(nameof(normalizedUserName));

            using (var db = DatabaseFactory.GetDatabase())
                return Task.FromResult(db.FirstOrDefault<TUser>("WHERE NormalizedUserName = @0", normalizedUserName));
        }

        public async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (login == null)
                throw new ArgumentNullException(nameof(login));

            var userLogin = new UserLogin(login, user.Id);

            using (var db = DatabaseFactory.GetDatabase())
                await db.InsertAsync(userLogin);
        }

        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentOutOfRangeException(nameof(loginProvider));
            if (string.IsNullOrWhiteSpace(providerKey))
                throw new ArgumentOutOfRangeException(nameof(providerKey));

            using (var db = DatabaseFactory.GetDatabase()) {
                db.DeleteWhere<UserLogin>("LoginProvider = @0 AND ProviderKey = @1 AND UserId = @2",
                    loginProvider, providerKey, user.Id);
            }

            return Task.FromResult(0);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            IList<UserLogin> logins;

            using (var db = DatabaseFactory.GetDatabase())
                logins = await db.FetchAsync<UserLogin>("WHERE UserId = @0", user.Id);

            cancellationToken.ThrowIfCancellationRequested();

            return logins.Select(l => l.ToLoginInfo()).ToArray();
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentOutOfRangeException(nameof(loginProvider));
            if (string.IsNullOrWhiteSpace(providerKey))
                throw new ArgumentOutOfRangeException(nameof(providerKey));

            var sql = Sql.Builder
                .Select($"{UserTableName}.*")
                .From(UserTableName)
                .InnerJoin(UserLoginTableName).On($"{UserTableName}.Id = {UserLoginTableName}.UserId")
                .Where($"{UserLoginTableName}.LoginProvider = @0 AND {UserLoginTableName}.ProviderKey = @1", loginProvider, providerKey);

            using (var db = DatabaseFactory.GetDatabase())
                return Task.FromResult(db.FirstOrDefault<TUser>(sql));
        }

        public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentOutOfRangeException(roleName);

            using (var db = DatabaseFactory.GetDatabase()) {
                var role = db.FirstOrDefault<IdentityRole>("WHERE Name = @0", roleName);
                if (role == null)
                    throw new InvalidOperationException($"Role {roleName} does not exist!");

                cancellationToken.ThrowIfCancellationRequested();

                var matchup = new UserRole(user.Id, role.Id);
                await db.InsertAsync(matchup);
            }
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentOutOfRangeException(roleName);

            using (var db = DatabaseFactory.GetDatabase()) {
                var role = db.FirstOrDefault<IdentityRole>("WHERE Name = @0", roleName);
                if (role == null)
                    throw new InvalidOperationException($"Role {roleName} does not exist!");

                cancellationToken.ThrowIfCancellationRequested();

                db.DeleteWhere<UserRole>("UserId = @0 AND RoleId = @1", user.Id, role.Id);
            }

            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var sql = Sql.Builder
                .Select($"{RoleTableName}.Name")
                .From(RoleTableName)
                .InnerJoin(UserRoleTableName).On($"{RoleTableName}.Id = {UserRoleTableName}.RoleId")
                .InnerJoin(UserTableName).On($"{UserRoleTableName}.UserId = {UserTableName}.Id")
                .Where($"{UserTableName}.Id = @0", user.Id);

            using (var db = DatabaseFactory.GetDatabase())
                return Task.FromResult((IList<string>)db.Fetch<string>(sql));
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentOutOfRangeException(nameof(roleName));

            var sql = Sql.Builder
                .Select($"COUNT({UserTableName}.Id)")
                .From(UserTableName)
                .InnerJoin(UserRoleTableName).On($"{UserTableName}.Id = {UserRoleTableName}.UserId")
                .InnerJoin(RoleTableName).On($"{UserRoleTableName}.RoleId = {RoleTableName}.Id")
                .Where($"{UserTableName}.Id = @0 AND {RoleTableName}.Name = @1", user.Id, roleName)
                .GroupBy($"{UserTableName}.Id");

            using (var db = DatabaseFactory.GetDatabase()) {
                var result = await db.ExecuteScalarAsync<int>(sql);
                return result > 0;
            }
        }

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentOutOfRangeException(nameof(roleName));

            var sql = Sql.Builder
                .Select($"{UserTableName}.*")
                .From(UserTableName)
                .InnerJoin(UserRoleTableName).On($"{UserTableName}.Id = {UserRoleTableName}.UserId")
                .InnerJoin(RoleTableName).On($"{UserRoleTableName}.RoleId = {RoleTableName}.Id")
                .Where($"{RoleTableName}.Name = @0", roleName);

            using (var db = DatabaseFactory.GetDatabase())
                return await db.FetchAsync<TUser>(sql);
        }

        public async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            IList<UserClaim> claims;

            using (var db = DatabaseFactory.GetDatabase())
                claims = await db.FetchAsync<UserClaim>("WHERE UserId = @0", user.Id);

            return claims.Select(c => c.ToClaim()).ToArray();
        }

        public async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            var sql = Sql.Builder
                .Select($"{UserTableName}.*")
                .From(UserTableName)
                .InnerJoin(UserClaimTableName)
                .On($"{UserTableName}.Id = {UserClaimTableName}.UserId")
                .Where("ClaimType = @0 AND ClaimValue = @1", claim.Type, claim.Value);

            using (var db = DatabaseFactory.GetDatabase())
                return await db.FetchAsync<TUser>(sql);
        }

        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            using (var db = DatabaseFactory.GetDatabase()) {
                foreach (var claim in claims) {
                    db.DeleteWhere<UserClaim>("UserId = @0 AND ClaimType = @1 AND ClaimValue = @2",
                        user.Id, claim.Type, claim.Value);
                }
            }

            return Task.FromResult(0);
        }

        public async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));
            if (newClaim == null)
                throw new ArgumentNullException(nameof(newClaim));

            await RemoveClaimsAsync(user, new[] { claim }, cancellationToken);
            await AddClaimsAsync(user, new[] { newClaim }, cancellationToken);
        }

        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            var saveClaims = claims.Select(c => new UserClaim(c, user.Id));

            using (var db = DatabaseFactory.GetDatabase())
                db.InsertBulk(saveClaims);

            return Task.FromResult(0);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentOutOfRangeException(nameof(passwordHash));

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var result = !string.IsNullOrWhiteSpace(user.PasswordHash);

            return Task.FromResult(result);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(stamp))
                throw new ArgumentOutOfRangeException(nameof(stamp));

            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentOutOfRangeException(nameof(email));

            user.Email = email;

            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.EmailConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(normalizedEmail))
                throw new ArgumentNullException(nameof(normalizedEmail));

            using (var db = DatabaseFactory.GetDatabase()) {
                var result = db.FirstOrDefault<TUser>("WHERE NormalizedEmail = @0", normalizedEmail);
                return Task.FromResult(result);
            }
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedEmail))
                throw new ArgumentOutOfRangeException(nameof(normalizedEmail));

            user.NormalizedEmail = normalizedEmail;

            return Task.FromResult(0);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.LockoutEnd = lockoutEnd;

            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(++user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.AccessFailedCount = 0;

            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.LockoutEnabled = enabled;

            return Task.FromResult(0);
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PhoneNumber = phoneNumber;

            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PhoneNumberConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.TwoFactorEnabled = enabled;

            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null) {
                throw new ArgumentNullException(nameof(user));
            }

            user.AuthenticatorKey = key;

            return Task.CompletedTask;
        }

        public Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null) {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.AuthenticatorKey);
        }

        public Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            throw new NotImplementedException();
            //if (user == null) {
            //    throw new ArgumentNullException(nameof(user));
            //}

            //user.RecoveryCodes = recoveryCodes;

            //return Task.CompletedTask;
        }

        public async Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            throw new NotImplementedException();

            //if (user == null) {
            //    throw new ArgumentNullException(nameof(user));
            //}

            //if (user.RecoveryCodes.Contains(code)) {
            //    var codes = user.RecoveryCodes.Where(x => x != code);
            //    await ReplaceCodesAsync(user, codes, cancellationToken);
            //    return true;
            //}

            //return false;
        }

        public Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            throw new NotImplementedException();

            //if (user == null) {
            //    throw new ArgumentNullException(nameof(user));
            //}

            //var recoveryCodesCount = user.RecoveryCodes?.Count();
            //return Task.FromResult(recoveryCodesCount ?? 0);
        }

        #region Disposal

        ~UserStore() {
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
