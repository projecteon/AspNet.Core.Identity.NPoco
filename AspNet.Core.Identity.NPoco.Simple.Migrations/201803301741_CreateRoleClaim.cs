﻿namespace AspNet.Core.Identity.NPoco.Simple.Migrations {
    using SimpleMigrations;

    [Migration(201803301741, "Create RoleClaim table")]
    public class CreateRoleClaim : Migration {

        protected override void Up() {
            Execute(@"CREATE TABLE RoleClaim (
                [Id] [int] IDENTITY(1,1) NOT NULL,
	            [ClaimType] [nvarchar](max) NULL,
	            [ClaimValue] [nvarchar](max) NULL,
	            [RoleId] [nvarchar](450) NULL,
             CONSTRAINT [PK_IdentityRoleClaim<string>] PRIMARY KEY CLUSTERED 
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");
        }

        protected override void Down() {
            Execute("DROP TABLE RoleClaim");
        }
    }
}
