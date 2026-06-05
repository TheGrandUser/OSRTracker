using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSRTracker.Migrations
{
    /// <inheritdoc />
    public partial class AttributeDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyAttributes",
                table: "ClassDefinitions");

            migrationBuilder.DropColumn(
                name: "AttributeNames",
                table: "CampaignSettings");

            migrationBuilder.CreateTable(
                name: "AttributeDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Ordinal = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClassKeyAttributes",
                columns: table => new
                {
                    ClassDefinitionId = table.Column<int>(type: "INTEGER", nullable: false),
                    KeyAttributesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassKeyAttributes", x => new { x.ClassDefinitionId, x.KeyAttributesId });
                    table.ForeignKey(
                        name: "FK_ClassKeyAttributes_AttributeDefinitions_KeyAttributesId",
                        column: x => x.KeyAttributesId,
                        principalTable: "AttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassKeyAttributes_ClassDefinitions_ClassDefinitionId",
                        column: x => x.ClassDefinitionId,
                        principalTable: "ClassDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassKeyAttributes_KeyAttributesId",
                table: "ClassKeyAttributes",
                column: "KeyAttributesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassKeyAttributes");

            migrationBuilder.DropTable(
                name: "AttributeDefinitions");

            migrationBuilder.AddColumn<string>(
                name: "KeyAttributes",
                table: "ClassDefinitions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttributeNames",
                table: "CampaignSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
