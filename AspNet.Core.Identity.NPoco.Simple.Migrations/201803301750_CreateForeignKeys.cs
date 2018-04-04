namespace AspNet.Core.Identity.NPoco.Simple.Migrations {
    using SimpleMigrations;

    [Migration(201803301750, "Create ForeignKeys")]
    public class CreateForeignKeys : Migration {

        protected override void Up() {
            Execute(@"ALTER TABLE [dbo].[RoleClaim]  WITH CHECK ADD  CONSTRAINT [FK_IdentityRoleClaim<string>_IdentityRole_RoleId] FOREIGN KEY([RoleId])
                    REFERENCES [dbo].[IdentityRole] ([Id])");
            Execute(@"ALTER TABLE [dbo].[RoleClaim] CHECK CONSTRAINT [FK_IdentityRoleClaim<string>_IdentityRole_RoleId]");
            Execute(@"ALTER TABLE [dbo].[UserClaim]  WITH CHECK ADD  CONSTRAINT [FK_IdentityUserClaim<string>_ApplicationUser_UserId] FOREIGN KEY([UserId])
                    REFERENCES [dbo].[IdentityUser] ([Id])");
            Execute(@"ALTER TABLE [dbo].[UserClaim] CHECK CONSTRAINT [FK_IdentityUserClaim<string>_ApplicationUser_UserId]");
            Execute(@"ALTER TABLE [dbo].[UserLogin]  WITH CHECK ADD  CONSTRAINT [FK_IdentityUserLogin<string>_ApplicationUser_UserId] FOREIGN KEY([UserId])
                    REFERENCES [dbo].[IdentityUser] ([Id])");
            Execute(@"ALTER TABLE [dbo].[UserLogin] CHECK CONSTRAINT [FK_IdentityUserLogin<string>_ApplicationUser_UserId]");
            Execute(@"ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_IdentityUserRole<string>_ApplicationUser_UserId] FOREIGN KEY([UserId])
                    REFERENCES [dbo].[IdentityUser] ([Id])");
            Execute(@"ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_IdentityUserRole<string>_ApplicationUser_UserId]");
            Execute(@"ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_IdentityUserRole<string>_IdentityRole_RoleId] FOREIGN KEY([RoleId])
                    REFERENCES [dbo].[IdentityRole] ([Id])");
            Execute(@"ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_IdentityUserRole<string>_IdentityRole_RoleId]");
        }

        protected override void Down() {
            Execute("ALTER TABLE [dbo].[RoleClaim] DROP CONSTRAINT [FK_IdentityRoleClaim<string>_IdentityRole_RoleId]");
            Execute("ALTER TABLE [dbo].[UserClaim] DROP CONSTRAINT [FK_IdentityUserClaim<string>_ApplicationUser_UserId]");
            Execute("ALTER TABLE [dbo].[UserLogin] DROP CONSTRAINT [FK_IdentityUserLogin<string>_ApplicationUser_UserId]");
            Execute("ALTER TABLE [dbo].[UserRole] DROP CONSTRAINT [FK_IdentityUserRole<string>_ApplicationUser_UserId]");
            Execute("ALTER TABLE [dbo].[UserRole] DROP CONSTRAINT [FK_IdentityUserRole<string>_IdentityRole_RoleId]");
        }
    }
}
