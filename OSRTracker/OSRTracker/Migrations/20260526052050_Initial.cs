using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSRTracker.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CampaignSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SystemName = table.Column<string>(type: "TEXT", nullable: false),
                    AttributeNames = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClassDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    KeyAttributes = table.Column<string>(type: "TEXT", nullable: true),
                    LevelXP = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    UnitValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    CountPerUnitWeight = table.Column<decimal>(type: "TEXT", nullable: false),
                    Ordinal = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Delves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LocationName = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Delves", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SessioNumber = table.Column<string>(type: "TEXT", nullable: false),
                    SessionNotes = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PlayerName = table.Column<string>(type: "TEXT", nullable: true),
                    CharacterType = table.Column<int>(type: "INTEGER", nullable: false),
                    ClassId = table.Column<int>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentXP = table.Column<int>(type: "INTEGER", nullable: false),
                    ShareMultiplierXP = table.Column<decimal>(type: "TEXT", nullable: false),
                    ShareMultiplierTreasure = table.Column<decimal>(type: "TEXT", nullable: false),
                    Str = table.Column<int>(type: "INTEGER", nullable: false),
                    Int = table.Column<int>(type: "INTEGER", nullable: false),
                    Wis = table.Column<int>(type: "INTEGER", nullable: false),
                    Dex = table.Column<int>(type: "INTEGER", nullable: false),
                    Con = table.Column<int>(type: "INTEGER", nullable: false),
                    Cha = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_ClassDefinitions_ClassId",
                        column: x => x.ClassId,
                        principalTable: "ClassDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SessionDelves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    DelveId = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionDelves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionDelves_Delves_DelveId",
                        column: x => x.DelveId,
                        principalTable: "Delves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionDelves_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionCharacters",
                columns: table => new
                {
                    CharactersId = table.Column<int>(type: "INTEGER", nullable: false),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionCharacters", x => new { x.CharactersId, x.SessionId });
                    table.ForeignKey(
                        name: "FK_SessionCharacters_Characters_CharactersId",
                        column: x => x.CharactersId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionCharacters_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralXPAwards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SessionDelveId = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralXPAwards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneralXPAwards_SessionDelves_SessionDelveId",
                        column: x => x.SessionDelveId,
                        principalTable: "SessionDelves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonsterEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SessionDelveId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    XPValue = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterEntries_SessionDelves_SessionDelveId",
                        column: x => x.SessionDelveId,
                        principalTable: "SessionDelves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreasureEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SessionDelveId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Quantiy = table.Column<int>(type: "INTEGER", nullable: false),
                    ApparentValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    Weight = table.Column<decimal>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    LocCharacterId = table.Column<int>(type: "INTEGER", nullable: true),
                    LocStore = table.Column<string>(type: "TEXT", nullable: true),
                    LocType = table.Column<int>(type: "INTEGER", nullable: false),
                    MagicItemDetails_IdentificationStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    MagicItemDetails_TrueValue = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreasureEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreasureEntries_SessionDelves_SessionDelveId",
                        column: x => x.SessionDelveId,
                        principalTable: "SessionDelves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterXPAward",
                columns: table => new
                {
                    CharactersId = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneralXPAwardId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterXPAward", x => new { x.CharactersId, x.GeneralXPAwardId });
                    table.ForeignKey(
                        name: "FK_CharacterXPAward_Characters_CharactersId",
                        column: x => x.CharactersId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterXPAward_GeneralXPAwards_GeneralXPAwardId",
                        column: x => x.GeneralXPAwardId,
                        principalTable: "GeneralXPAwards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_ClassId",
                table: "Characters",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterXPAward_GeneralXPAwardId",
                table: "CharacterXPAward",
                column: "GeneralXPAwardId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralXPAwards_SessionDelveId",
                table: "GeneralXPAwards",
                column: "SessionDelveId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterEntries_SessionDelveId",
                table: "MonsterEntries",
                column: "SessionDelveId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionCharacters_SessionId",
                table: "SessionCharacters",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionDelves_DelveId",
                table: "SessionDelves",
                column: "DelveId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionDelves_SessionId_DelveId",
                table: "SessionDelves",
                columns: new[] { "SessionId", "DelveId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreasureEntries_SessionDelveId",
                table: "TreasureEntries",
                column: "SessionDelveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignSettings");

            migrationBuilder.DropTable(
                name: "CharacterXPAward");

            migrationBuilder.DropTable(
                name: "CurrencyDefinitions");

            migrationBuilder.DropTable(
                name: "MonsterEntries");

            migrationBuilder.DropTable(
                name: "SessionCharacters");

            migrationBuilder.DropTable(
                name: "TreasureEntries");

            migrationBuilder.DropTable(
                name: "GeneralXPAwards");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "SessionDelves");

            migrationBuilder.DropTable(
                name: "ClassDefinitions");

            migrationBuilder.DropTable(
                name: "Delves");

            migrationBuilder.DropTable(
                name: "Sessions");
        }
    }
}
