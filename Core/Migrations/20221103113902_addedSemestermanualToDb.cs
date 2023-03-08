using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    public partial class addedSemestermanualToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SemesterManual",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BibleVerse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpeakerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CordinatorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemesterManual", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SemesterManual_AspNetUsers_CordinatorId",
                        column: x => x.CordinatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SemesterManual_AspNetUsers_SpeakerId",
                        column: x => x.SpeakerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SemesterManual_School_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "School",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SemesterManual_CordinatorId",
                table: "SemesterManual",
                column: "CordinatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SemesterManual_SchoolId",
                table: "SemesterManual",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_SemesterManual_SpeakerId",
                table: "SemesterManual",
                column: "SpeakerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SemesterManual");
        }
    }
}
