using Microsoft.EntityFrameworkCore.Migrations;
using HomeCareAppointment.DAL;

#nullable disable

namespace HomeCareAppointment.Migrations
{
    /// <inheritdoc />
    public partial class InitFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patient_PatientId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AvailableDayId",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patient",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "AvailableDays");

            migrationBuilder.RenameTable(
                name: "Patient",
                newName: "Patients");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AvailableDayId",
                table: "Appointments",
                column: "AvailableDayId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AvailableDayId",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.RenameTable(
                name: "Patients",
                newName: "Patient");

            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "AvailableDays",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patient",
                table: "Patient",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AvailableDayId",
                table: "Appointments",
                column: "AvailableDayId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patient_PatientId",
                table: "Appointments",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "PatientId");
        }
    }
}
