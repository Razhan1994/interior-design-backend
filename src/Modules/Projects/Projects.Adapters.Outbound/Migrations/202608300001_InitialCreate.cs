using Microsoft.EntityFrameworkCore.Migrations;

namespace InteriorMarketplace.Modules.Projects.Adapters.Outbound.Migrations;

[Migration("202608300001_InitialCreate")]
public sealed class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema("marketplace");
        CreateProjectsTable(migrationBuilder);
        CreateOffersTable(migrationBuilder);
        CreateProjectElementsTable(migrationBuilder);
        CreateIndexes(migrationBuilder);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "offers", schema: "marketplace");
        migrationBuilder.DropTable(name: "project_elements", schema: "marketplace");
        migrationBuilder.DropTable(name: "projects", schema: "marketplace");
    }

    private static void CreateProjectsTable(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "projects",
            schema: "marketplace",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                OwnerId = table.Column<Guid>(nullable: false),
                Title = table.Column<string>(maxLength: 200, nullable: false),
                RoomImageUrl = table.Column<string>(maxLength: 1000, nullable: false),
                Status = table.Column<int>(nullable: false),
                CreatedAtUtc = table.Column<DateTime>(nullable: false),
                PublishedAtUtc = table.Column<DateTime>(nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_projects", row => row.Id));
    }

    private static void CreateOffersTable(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "offers",
            schema: "marketplace",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                ProjectId = table.Column<Guid>(nullable: false),
                ElementId = table.Column<Guid>(nullable: false),
                VendorId = table.Column<Guid>(nullable: false),
                Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                DeliveryDays = table.Column<int>(nullable: false),
                Note = table.Column<string>(nullable: true),
                ProductImageUrl = table.Column<string>(nullable: true),
                Status = table.Column<int>(nullable: false),
                CreatedAtUtc = table.Column<DateTime>(nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_offers", row => row.Id));
    }

    private static void CreateProjectElementsTable(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "project_elements",
            schema: "marketplace",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                ProjectId = table.Column<Guid>(nullable: false),
                Category = table.Column<string>(maxLength: 100, nullable: false),
                Title = table.Column<string>(maxLength: 200, nullable: false),
                Description = table.Column<string>(nullable: true),
                Dimensions = table.Column<string>(nullable: true),
                Color = table.Column<string>(nullable: true),
                TargetBudget = table.Column<decimal>(nullable: true),
                Rectangle_X = table.Column<decimal>(nullable: false),
                Rectangle_Y = table.Column<decimal>(nullable: false),
                Rectangle_Width = table.Column<decimal>(nullable: false),
                Rectangle_Height = table.Column<decimal>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_project_elements", row => row.Id);
                table.ForeignKey(
                    name: "FK_elements_projects",
                    column: row => row.ProjectId,
                    principalSchema: "marketplace",
                    principalTable: "projects",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    private static void CreateIndexes(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_offers_VendorId_ElementId",
            schema: "marketplace",
            table: "offers",
            columns: ["VendorId", "ElementId"],
            unique: true,
            filter: "\"Status\" = 0");

        migrationBuilder.CreateIndex(
            name: "IX_project_elements_ProjectId",
            schema: "marketplace",
            table: "project_elements",
            column: "ProjectId");
    }
}
