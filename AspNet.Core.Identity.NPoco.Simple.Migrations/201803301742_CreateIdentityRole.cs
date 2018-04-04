namespace AspNet.Core.Identity.NPoco.Simple.Migrations {
    using SimpleMigrations;

    [Migration(201803301742, "Create IdentityRole table")]
    public class CreateIdentityRole : Migration {

        protected override void Up() {
            Execute(@"CREATE TABLE IdentityRole (
                [Id] [CHAR](36) NOT NULL PRIMARY KEY,
                [Name] [VARCHAR](250) NOT NULL,
                [NormalizedName] [nvarchar](256) NULL,
                CONSTRAINT [PK_IdentityRole] PRIMARY KEY CLUSTERED 
                (
	                [Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");
        }

        protected override void Down() {
            Execute("DROP TABLE IdentityRole");
        }
    }
}
