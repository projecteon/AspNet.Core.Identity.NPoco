namespace AspNet.Core.Identity.NPoco.Simple.Migrations {
    using SimpleMigrations;

    [Migration(201803301744, "Create UserLogin table")]
    public class CreateUserLogin : Migration {

        protected override void Up() {
            Execute(@"CREATE TABLE UserLogin (
                [LoginProvider] [nvarchar](450) NOT NULL,
	            [ProviderKey] [nvarchar](450) NOT NULL,
	            [ProviderDisplayName] [nvarchar](max) NULL,
	            [UserId] [nvarchar](450) NULL,
             CONSTRAINT [PK_IdentityUserLogin<string>] PRIMARY KEY CLUSTERED 
            (
	            [LoginProvider] ASC,
	            [ProviderKey] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");
        }

        protected override void Down() {
            Execute("DROP TABLE UserLogin");
        }
    }
}
