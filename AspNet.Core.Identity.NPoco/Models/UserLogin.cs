using Microsoft.AspNetCore.Identity;

namespace AspNet.Core.Identity.NPoco.Models {
    public class UserLogin {
        private UserLogin() { }

        public UserLogin(UserLoginInfo loginInfo, string userId) {
            LoginProvider = loginInfo.LoginProvider;
            ProviderDisplayName = loginInfo.ProviderDisplayName;
            ProviderKey = loginInfo.ProviderKey;
            UserId = userId;
        }

        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
        public string UserId { get; set; }

        public UserLoginInfo ToLoginInfo() {
            return new UserLoginInfo(LoginProvider, ProviderKey, ProviderDisplayName);
        }
    }
}
