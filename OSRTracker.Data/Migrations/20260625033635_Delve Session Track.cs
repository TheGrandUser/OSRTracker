using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSRTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class DelveSessionTrack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionTrackCharacter_Characters_CharacterId",
                table: "SessionTrackCharacter");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionTrackCharacter_SessionTracks_SessionTrackId",
                table: "SessionTrackCharacter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SessionTrackCharacter",
                table: "SessionTrackCharacter");

            migrationBuilder.RenameTable(
                name: "SessionTrackCharacter",
                newName: "SessionTracksCharacters");

            migrationBuilder.RenameIndex(
                name: "IX_SessionTrackCharacter_CharacterId",
                table: "SessionTracksCharacters",
                newName: "IX_SessionTracksCharacters_CharacterId");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Sessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SessionTrackId",
                table: "Delves",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SessionTracksCharacters",
                table: "SessionTracksCharacters",
                columns: new[] { "SessionTrackId", "CharacterId" });

            migrationBuilder.CreateIndex(
                name: "IX_Delves_SessionTrackId",
                table: "Delves",
                column: "SessionTrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Delves_SessionTracks_SessionTrackId",
                table: "Delves",
                column: "SessionTrackId",
                principalTable: "SessionTracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionTracksCharacters_Characters_CharacterId",
                table: "SessionTracksCharacters",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionTracksCharacters_SessionTracks_SessionTrackId",
                table: "SessionTracksCharacters",
                column: "SessionTrackId",
                principalTable: "SessionTracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Delves_SessionTracks_SessionTrackId",
                table: "Delves");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionTracksCharacters_Characters_CharacterId",
                table: "SessionTracksCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionTracksCharacters_SessionTracks_SessionTrackId",
                table: "SessionTracksCharacters");

            migrationBuilder.DropIndex(
                name: "IX_Delves_SessionTrackId",
                table: "Delves");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SessionTracksCharacters",
                table: "SessionTracksCharacters");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "SessionTrackId",
                table: "Delves");

            migrationBuilder.RenameTable(
                name: "SessionTracksCharacters",
                newName: "SessionTrackCharacter");

            migrationBuilder.RenameIndex(
                name: "IX_SessionTracksCharacters_CharacterId",
                table: "SessionTrackCharacter",
                newName: "IX_SessionTrackCharacter_CharacterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SessionTrackCharacter",
                table: "SessionTrackCharacter",
                columns: new[] { "SessionTrackId", "CharacterId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SessionTrackCharacter_Characters_CharacterId",
                table: "SessionTrackCharacter",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionTrackCharacter_SessionTracks_SessionTrackId",
                table: "SessionTrackCharacter",
                column: "SessionTrackId",
                principalTable: "SessionTracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
