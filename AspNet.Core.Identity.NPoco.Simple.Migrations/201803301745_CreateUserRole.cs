namespace AspNet.Core.Identity.NPoco.Simple.Migrations {
    using SimpleMigrations;

    [Migration(201803301745, "Create UserRole table")]
    public class CreateUserRole : Migration {

        protected override void Up() {
            Execute(@"CREATE TABLE UserRole (
                [Id] [nvarchar](450) NOT NULL,
	            [ConcurrencyStamp] [nvarchar](max) NULL,
	            [Name] [nvarchar](256) NULL,
	            [NormalizedName] [nvarchar](256) NULL,
             CONSTRAINT [PK_IdentityRole] PRIMARY KEY CLUSTERED 
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");
        }

        protected override void Down() {
            Execute("DROP TABLE UserRole");
        }
    }
}
