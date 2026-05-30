using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicMvc.Migrations
{
    /// <inheritdoc />
    public partial class DoctorWorkHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkEndHour",
                table: "Doctors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkStartHour",
                table: "Doctors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkEndHour",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "WorkStartHour",
                table: "Doctors");
        }
    }
}
