namespace AspNet.Core.Identity.NPoco.Simple.Migrations {
    using SimpleMigrations;

    [Migration(201803301748, "Create EmailIndex")]
    public class CreateEmailIndex : Migration {

        protected override void Up() {
            Execute(@"CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[IdentityUser]
                    (
	                    [NormalizedEmail] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        }

        protected override void Down() {
            Execute("DROP INDEX [EmailIndex] ON [dbo].[IdentityUser]");
        }
    }
}
