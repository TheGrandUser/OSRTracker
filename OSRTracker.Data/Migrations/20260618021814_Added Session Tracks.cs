using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSRTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedSessionTracks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SessionTrackId",
                table: "Sessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LocationDescription",
                table: "Delves",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SessionTracks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    GroupDescription = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionTracks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionTrackCharacter",
                columns: table => new
                {
                    SessionTrackId = table.Column<int>(type: "INTEGER", nullable: false),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionTrackCharacter", x => new { x.SessionTrackId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_SessionTrackCharacter_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionTrackCharacter_SessionTracks_SessionTrackId",
                        column: x => x.SessionTrackId,
                        principalTable: "SessionTracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SessionTrackId",
                table: "Sessions",
                column: "SessionTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionTrackCharacter_CharacterId",
                table: "SessionTrackCharacter",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_SessionTracks_SessionTrackId",
                table: "Sessions",
                column: "SessionTrackId",
                principalTable: "SessionTracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_SessionTracks_SessionTrackId",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "SessionTrackCharacter");

            migrationBuilder.DropTable(
                name: "SessionTracks");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_SessionTrackId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "SessionTrackId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "LocationDescription",
                table: "Delves");
        }
    }
}
