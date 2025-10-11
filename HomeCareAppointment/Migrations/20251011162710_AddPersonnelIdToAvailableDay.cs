using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeCareAppointment.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonnelIdToAvailableDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonnelId",
                table: "AvailableDays",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Personnels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personnels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvailableDays_PersonnelId",
                table: "AvailableDays",
                column: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AvailableDays_Personnels_PersonnelId",
                table: "AvailableDays",
                column: "PersonnelId",
                principalTable: "Personnels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvailableDays_Personnels_PersonnelId",
                table: "AvailableDays");

            migrationBuilder.DropTable(
                name: "Personnels");

            migrationBuilder.DropIndex(
                name: "IX_AvailableDays_PersonnelId",
                table: "AvailableDays");

            migrationBuilder.DropColumn(
                name: "PersonnelId",
                table: "AvailableDays");
        }
    }
}
