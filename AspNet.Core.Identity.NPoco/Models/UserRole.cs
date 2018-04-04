namespace AspNet.Core.Identity.NPoco.Models {
    public class UserRole {
        private UserRole() {
        }

        public UserRole(string userId, string roleId) {
            UserId = userId;
            RoleId = roleId;
        }

        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}
