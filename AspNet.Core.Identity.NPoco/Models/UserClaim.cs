namespace AspNet.Core.Identity.NPoco.Models {
    using System;
    using System.Security.Claims;

    public class UserClaim {
        private UserClaim() {
            Id = Guid.NewGuid().ToString();
        }

        public UserClaim(Claim claim, string userId) : this() {
            UserId = userId;
            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public Claim ToClaim() {
            return new Claim(ClaimType, ClaimValue);
        }
    }
}
