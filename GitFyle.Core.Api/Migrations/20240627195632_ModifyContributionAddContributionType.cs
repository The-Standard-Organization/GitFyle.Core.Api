using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GitFyle.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class ModifyContributionAddContributionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Contributions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_ContributionTypeId",
                table: "Contributions",
                column: "ContributionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contributions_ContributionTypes_ContributionTypeId",
                table: "Contributions",
                column: "ContributionTypeId",
                principalTable: "ContributionTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contributions_ContributionTypes_ContributionTypeId",
                table: "Contributions");

            migrationBuilder.DropIndex(
                name: "IX_Contributions_ContributionTypeId",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Contributions");
        }
    }
}
