using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSRTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedSessionCharacterandDelveCharacter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionCharacters_Characters_CharactersId",
                table: "SessionCharacters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SessionCharacters",
                table: "SessionCharacters");

            migrationBuilder.DropIndex(
                name: "IX_SessionCharacters_SessionId",
                table: "SessionCharacters");

            migrationBuilder.DropColumn(
                name: "MagicItemDetails_TrueValue",
                table: "TreasureEntries");

            migrationBuilder.RenameColumn(
                name: "ApparentValue",
                table: "TreasureEntries",
                newName: "MagicItemDetails_ApparentValue");

            migrationBuilder.RenameColumn(
                name: "CharactersId",
                table: "SessionCharacters",
                newName: "CharacterId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TreasureEntries",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MagicItemDetails_ApparentValue",
                table: "TreasureEntries",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationStatus",
                table: "TreasureEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleStatus",
                table: "TreasureEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "TreasureEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SessionTracks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Sessions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SessionDelves",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Applied",
                table: "SessionCharacters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "MonsterEntries",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<bool>(
                name: "HasBeenApplied",
                table: "MonsterEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "GeneralXPAwards",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<bool>(
                name: "HasBeenApplied",
                table: "GeneralXPAwards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Delves",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CurrencyDefinitions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClassDefinitions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Characters",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<decimal>(
                name: "XPBonus",
                table: "Characters",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CampaignSettings",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "DelveCalcMethod",
                table: "CampaignSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "XPForFirstLevel",
                table: "CampaignSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AttributeDefinitions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SessionCharacters",
                table: "SessionCharacters",
                columns: new[] { "SessionId", "CharacterId" });

            migrationBuilder.CreateTable(
                name: "DelveCharacter",
                columns: table => new
                {
                    DelveId = table.Column<int>(type: "INTEGER", nullable: false),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                    AppliedXP = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelveCharacter", x => new { x.DelveId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_DelveCharacter_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DelveCharacter_Delves_DelveId",
                        column: x => x.DelveId,
                        principalTable: "Delves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionCharacters_CharacterId",
                table: "SessionCharacters",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_DelveCharacter_CharacterId",
                table: "DelveCharacter",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionCharacters_Characters_CharacterId",
                table: "SessionCharacters",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionCharacters_Characters_CharacterId",
                table: "SessionCharacters");

            migrationBuilder.DropTable(
                name: "DelveCharacter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SessionCharacters",
                table: "SessionCharacters");

            migrationBuilder.DropIndex(
                name: "IX_SessionCharacters_CharacterId",
                table: "SessionCharacters");

            migrationBuilder.DropColumn(
                name: "ApplicationStatus",
                table: "TreasureEntries");

            migrationBuilder.DropColumn(
                name: "SaleStatus",
                table: "TreasureEntries");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "TreasureEntries");

            migrationBuilder.DropColumn(
                name: "AppliedXP",
                table: "SessionCharacters");

            migrationBuilder.DropColumn(
                name: "HasBeenApplied",
                table: "MonsterEntries");

            migrationBuilder.DropColumn(
                name: "HasBeenApplied",
                table: "GeneralXPAwards");

            migrationBuilder.DropColumn(
                name: "XPBonus",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "DelveCalcMethod",
                table: "CampaignSettings");

            migrationBuilder.DropColumn(
                name: "XPForFirstLevel",
                table: "CampaignSettings");

            migrationBuilder.RenameColumn(
                name: "MagicItemDetails_ApparentValue",
                table: "TreasureEntries",
                newName: "ApparentValue");

            migrationBuilder.RenameColumn(
                name: "CharacterId",
                table: "SessionCharacters",
                newName: "CharactersId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TreasureEntries",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ApparentValue",
                table: "TreasureEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MagicItemDetails_TrueValue",
                table: "TreasureEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SessionTracks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Sessions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SessionDelves",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "MonsterEntries",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "GeneralXPAwards",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Delves",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CurrencyDefinitions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClassDefinitions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Characters",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CampaignSettings",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AttributeDefinitions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SessionCharacters",
                table: "SessionCharacters",
                columns: new[] { "CharactersId", "SessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_SessionCharacters_SessionId",
                table: "SessionCharacters",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionCharacters_Characters_CharactersId",
                table: "SessionCharacters",
                column: "CharactersId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
