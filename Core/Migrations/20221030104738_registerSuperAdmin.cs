using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    public partial class registerSuperAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
               $"INSERT INTO AspNetRoles VALUES ('0DB45C30-2FEE-47C6-AF34-7849A62B8856','SuperAdmin','SUPERADMIN','0DB45C30-2FEE-47C6-AF34-7849A62B8856')" +
               $"INSERT INTO AspNetRoles VALUES ('4e3b804b-59a8-49d0-bf8b-2c71e46e7921','SchoolAdmin','SchoolAdmin',NEWID())" +
               $"INSERT INTO AspNetRoles VALUES ('b7ee852b-fecd-4a82-ba16-40aba1fbcf28','User','User',NEWID())");

            migrationBuilder.Sql(
                $"INSERT INTO [dbo].[AspNetUsers] ([Id], [Discriminator],[FirstName],[MiddleName], [LastName],[DateOfBirth], [GenderId],[StateId],[HomeAddress],[SchoolAddress], [SchoolId],[ReligionId],[NationalityId],[HomeSynagogue],[Department]," +
                $"[ProfilePicture],[DateRegistered],[Deactivated],[BlogId],[VerificationStatus],[ContactName],[ContactRelationship],[ContactPhoneNumber]," +
                $"[UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed]," +
                $"[PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount])" +
                $"VALUES(N'65de30ed-e458-4557-a1ac-dcdee04d8660', N'ApplicationUser', NULL, NULL, NULL, NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL," +
                $"NULL,GETDATE(),0,NULL,1,NULL,NULL,NULL," +
                $"N'covenantstudentsfellowship@gmail.com', N'COVENANTSTUDENTSFELLOWSHIP@GMAIL.COM', N'covenantstudentsfellowship@gmail.com', N'COVENANTSTUDENTSFELLOWSHIP@GMAIL.COM', 1," +
                $"N'AQAAAAEAACcQAAAAEI9tysdZONvrct+Xtn3rED46Y1+VSxdPRySQtoVhlOSDPbxup8FONZFBnRWb4q8d6w==', N'ZX7PRON25IHCM663CWZN2YPADPCTIXP2', N'7116441b-637a-41e2-b842-c8ac2cf3e5eb', NULL, 0,0,NULL,1,0)");

            migrationBuilder.Sql($"INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'65de30ed-e458-4557-a1ac-dcdee04d8660', N'0DB45C30-2FEE-47C6-AF34-7849A62B8856')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
