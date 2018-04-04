namespace AspNet.Core.Identity.NPoco.Models {
    using System;
    using System.Collections.Generic;

    public class IdentityUser {
        private IdentityUser() {
            Id = Guid.NewGuid().ToString();
        }

        public IdentityUser(string userName) : this() {
            UserName = userName;
        }

        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string AuthenticatorKey { get; set; }

        public override string ToString() {
            return UserName;
        }
    }
}
