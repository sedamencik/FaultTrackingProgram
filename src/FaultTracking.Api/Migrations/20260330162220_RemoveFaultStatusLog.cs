using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FaultTracking.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFaultStatusLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FaultStatusLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FaultStatusLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChangedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FaultReportId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    OldStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaultStatusLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaultStatusLogs_FaultReports_FaultReportId",
                        column: x => x.FaultReportId,
                        principalTable: "FaultReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaultStatusLogs_Users_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FaultStatusLogs_ChangedById",
                table: "FaultStatusLogs",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_FaultStatusLogs_FaultReportId",
                table: "FaultStatusLogs",
                column: "FaultReportId");
        }
    }
}
