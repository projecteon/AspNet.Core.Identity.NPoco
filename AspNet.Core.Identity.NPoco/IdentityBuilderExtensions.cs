namespace AspNet.Core.Identity.NPoco {
    using AspNet.Core.Identity.NPoco.Models;
    using AspNet.Core.Identity.NPoco.Stores;
    using Microsoft.AspNetCore.Identity;

    public static class IdentityBuilderExtensions {
        public static IdentityBuilder AddNPocoStores(this IdentityBuilder builder) {

            return builder
                .AddUserStore<UserStore<IdentityUser, IdentityRole, UserClaim, UserLogin, UserRole>>()
                .AddRoleStore<RoleStore<IdentityRole>>();
        }

        public static IdentityBuilder AddNPocoStores<TUser, TRole>(this IdentityBuilder builder)
            where TUser : IdentityUser
            where TRole : IdentityRole {

            return builder
                .AddUserStore<UserStore<TUser, TRole, UserClaim, UserLogin, UserRole>>()
                .AddRoleStore<RoleStore<TRole>>();
        }

        public static IdentityBuilder AddNPocoStores<TUser, TRole, TUserClaim, TUserLogin, TUserRole>(
            this IdentityBuilder builder)
            where TUser : IdentityUser
            where TRole : IdentityRole
            where TUserClaim : UserClaim
            where TUserLogin : UserLogin
            where TUserRole : UserRole {

            return builder
                .AddUserStore<UserStore<TUser, TRole, TUserClaim, TUserLogin, TUserRole>>()
                .AddRoleStore<RoleStore<TRole>>();
        }
    }
}
