using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project3.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "cat_id",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    cat_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.cat_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_cat_id",
                table: "Jobs",
                column: "cat_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Categories_cat_id",
                table: "Jobs",
                column: "cat_id",
                principalTable: "Categories",
                principalColumn: "cat_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Categories_cat_id",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_cat_id",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "cat_id",
                table: "Jobs");
        }
    }
}
