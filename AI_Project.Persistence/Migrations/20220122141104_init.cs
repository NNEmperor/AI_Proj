using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AI_Project.Persistence.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Data",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTimeOfMeasurement = table.Column<DateTime>(nullable: false),
                    Temperature = table.Column<float>(nullable: false),
                    Cloudiness = table.Column<float>(nullable: false),
                    Humidity = table.Column<float>(nullable: false),
                    WindSpeed = table.Column<float>(nullable: false),
                    ElectricSpending = table.Column<float>(nullable: false),
                    DayOfTheWeek = table.Column<float>(nullable: false),
                    MonthOfTheYear = table.Column<float>(nullable: false),
                    TimeOfDay = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Data");
        }
    }
}
