using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StateInfos",
                columns: table => new
                {
                    StateInfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateInfos", x => x.StateInfoId);
                });

            migrationBuilder.CreateTable(
                name: "AdInfos",
                columns: table => new
                {
                    AdInfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateInfoId = table.Column<int>(type: "int", nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdInfos", x => x.AdInfoId);
                    table.ForeignKey(
                        name: "FK_AdInfos_StateInfos_StateInfoId",
                        column: x => x.StateInfoId,
                        principalTable: "StateInfos",
                        principalColumn: "StateInfoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdDetailInfos",
                columns: table => new
                {
                    AdDetailInfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateInfoId = table.Column<int>(type: "int", nullable: false),
                    AdInfoId = table.Column<int>(type: "int", nullable: false),
                    TitleVi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentVi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInfo1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInfo2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInfo3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Images = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdDetailInfos", x => x.AdDetailInfoId);
                    table.ForeignKey(
                        name: "FK_AdDetailInfos_AdInfos_AdInfoId",
                        column: x => x.AdInfoId,
                        principalTable: "AdInfos",
                        principalColumn: "AdInfoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdDetailInfos_StateInfos_StateInfoId",
                        column: x => x.StateInfoId,
                        principalTable: "StateInfos",
                        principalColumn: "StateInfoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdDetailInfos_AdInfoId",
                table: "AdDetailInfos",
                column: "AdInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdDetailInfos_StateInfoId",
                table: "AdDetailInfos",
                column: "StateInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_AdInfos_StateInfoId",
                table: "AdInfos",
                column: "StateInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdDetailInfos");

            migrationBuilder.DropTable(
                name: "AdInfos");

            migrationBuilder.DropTable(
                name: "StateInfos");
        }
    }
}
