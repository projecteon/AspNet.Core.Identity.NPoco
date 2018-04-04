namespace AspNet.Core.Identity.NPoco.Simple.Migrations {
    using SimpleMigrations;

    [Migration(201803301749, "Create UserNameIndex")]
    public class CreateUserNameIndex : Migration {

        protected override void Up() {
            Execute(@"CREATE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[IdentityUser]
                    (
	                    [NormalizedUserName] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        }

        protected override void Down() {
            Execute("DROP INDEX [UserNameIndex] ON [dbo].[IdentityUser]");
        }
    }
}
