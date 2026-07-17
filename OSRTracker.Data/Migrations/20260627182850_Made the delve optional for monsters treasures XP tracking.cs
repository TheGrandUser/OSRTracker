using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSRTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class MadethedelveoptionalformonsterstreasuresXPtracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeneralXPAwards_SessionDelves_SessionDelveId",
                table: "GeneralXPAwards");

            migrationBuilder.DropForeignKey(
                name: "FK_MonsterEntries_SessionDelves_SessionDelveId",
                table: "MonsterEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_TreasureEntries_SessionDelves_SessionDelveId",
                table: "TreasureEntries");

            migrationBuilder.RenameColumn(
                name: "SessionDelveId",
                table: "TreasureEntries",
                newName: "SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_TreasureEntries_SessionDelveId",
                table: "TreasureEntries",
                newName: "IX_TreasureEntries_SessionId");

            migrationBuilder.RenameColumn(
                name: "SessionDelveId",
                table: "MonsterEntries",
                newName: "SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_MonsterEntries_SessionDelveId",
                table: "MonsterEntries",
                newName: "IX_MonsterEntries_SessionId");

            migrationBuilder.RenameColumn(
                name: "SessionDelveId",
                table: "GeneralXPAwards",
                newName: "SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_GeneralXPAwards_SessionDelveId",
                table: "GeneralXPAwards",
                newName: "IX_GeneralXPAwards_SessionId");

            migrationBuilder.AddColumn<int>(
                name: "DelveId",
                table: "TreasureEntries",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DelveId",
                table: "MonsterEntries",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DelveId",
                table: "GeneralXPAwards",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreasureEntries_DelveId",
                table: "TreasureEntries",
                column: "DelveId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterEntries_DelveId",
                table: "MonsterEntries",
                column: "DelveId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralXPAwards_DelveId",
                table: "GeneralXPAwards",
                column: "DelveId");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralXPAwards_Delves_DelveId",
                table: "GeneralXPAwards",
                column: "DelveId",
                principalTable: "Delves",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralXPAwards_Sessions_SessionId",
                table: "GeneralXPAwards",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonsterEntries_Delves_DelveId",
                table: "MonsterEntries",
                column: "DelveId",
                principalTable: "Delves",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MonsterEntries_Sessions_SessionId",
                table: "MonsterEntries",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TreasureEntries_Delves_DelveId",
                table: "TreasureEntries",
                column: "DelveId",
                principalTable: "Delves",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TreasureEntries_Sessions_SessionId",
                table: "TreasureEntries",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeneralXPAwards_Delves_DelveId",
                table: "GeneralXPAwards");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneralXPAwards_Sessions_SessionId",
                table: "GeneralXPAwards");

            migrationBuilder.DropForeignKey(
                name: "FK_MonsterEntries_Delves_DelveId",
                table: "MonsterEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_MonsterEntries_Sessions_SessionId",
                table: "MonsterEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_TreasureEntries_Delves_DelveId",
                table: "TreasureEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_TreasureEntries_Sessions_SessionId",
                table: "TreasureEntries");

            migrationBuilder.DropIndex(
                name: "IX_TreasureEntries_DelveId",
                table: "TreasureEntries");

            migrationBuilder.DropIndex(
                name: "IX_MonsterEntries_DelveId",
                table: "MonsterEntries");

            migrationBuilder.DropIndex(
                name: "IX_GeneralXPAwards_DelveId",
                table: "GeneralXPAwards");

            migrationBuilder.DropColumn(
                name: "DelveId",
                table: "TreasureEntries");

            migrationBuilder.DropColumn(
                name: "DelveId",
                table: "MonsterEntries");

            migrationBuilder.DropColumn(
                name: "DelveId",
                table: "GeneralXPAwards");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "TreasureEntries",
                newName: "SessionDelveId");

            migrationBuilder.RenameIndex(
                name: "IX_TreasureEntries_SessionId",
                table: "TreasureEntries",
                newName: "IX_TreasureEntries_SessionDelveId");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "MonsterEntries",
                newName: "SessionDelveId");

            migrationBuilder.RenameIndex(
                name: "IX_MonsterEntries_SessionId",
                table: "MonsterEntries",
                newName: "IX_MonsterEntries_SessionDelveId");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "GeneralXPAwards",
                newName: "SessionDelveId");

            migrationBuilder.RenameIndex(
                name: "IX_GeneralXPAwards_SessionId",
                table: "GeneralXPAwards",
                newName: "IX_GeneralXPAwards_SessionDelveId");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralXPAwards_SessionDelves_SessionDelveId",
                table: "GeneralXPAwards",
                column: "SessionDelveId",
                principalTable: "SessionDelves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonsterEntries_SessionDelves_SessionDelveId",
                table: "MonsterEntries",
                column: "SessionDelveId",
                principalTable: "SessionDelves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TreasureEntries_SessionDelves_SessionDelveId",
                table: "TreasureEntries",
                column: "SessionDelveId",
                principalTable: "SessionDelves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
