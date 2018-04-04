namespace AspNet.Core.Identity.NPoco.Simple.Migrations {
    using SimpleMigrations;

    [Migration(201803301746, "Create IdentityUser table")]
    public class CreateIdentityUser : Migration {

        protected override void Up() {
            Execute(@"CREATE TABLE IdentityUser (
                [Id] [nvarchar](450) NOT NULL,
	            [AccessFailedCount] [int] NOT NULL,
	            [ConcurrencyStamp] [nvarchar](max) NULL,
	            [Email] [nvarchar](256) NULL,
	            [EmailConfirmed] [bit] NOT NULL,
	            [LockoutEnabled] [bit] NOT NULL,
	            [LockoutEnd] [datetimeoffset](7) NULL,
	            [NormalizedEmail] [nvarchar](256) NULL,
	            [NormalizedUserName] [nvarchar](256) NULL,
	            [PasswordHash] [nvarchar](max) NULL,
	            [PhoneNumber] [nvarchar](max) NULL,
	            [PhoneNumberConfirmed] [bit] NOT NULL,
	            [SecurityStamp] [nvarchar](max) NULL,
	            [TwoFactorEnabled] [bit] NOT NULL,
	            [UserName] [nvarchar](256) NULL,
	            [AuthenticatorKey] [nvarchar](256) NULL,    
             CONSTRAINT [PK_ApplicationUser] PRIMARY KEY CLUSTERED 
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");
        }

        protected override void Down() {
            Execute("DROP TABLE IdentityUser");
        }
    }
}
