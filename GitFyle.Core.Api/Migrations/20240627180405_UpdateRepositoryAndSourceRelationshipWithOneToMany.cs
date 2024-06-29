using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GitFyle.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRepositoryAndSourceRelationshipWithOneToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repositories_Sources_SourceId",
                table: "Repositories");

            migrationBuilder.DropIndex(
                name: "IX_Repositories_SourceId",
                table: "Repositories");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_SourceId",
                table: "Repositories",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Repositories_Sources_SourceId",
                table: "Repositories",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repositories_Sources_SourceId",
                table: "Repositories");

            migrationBuilder.DropIndex(
                name: "IX_Repositories_SourceId",
                table: "Repositories");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_SourceId",
                table: "Repositories",
                column: "SourceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Repositories_Sources_SourceId",
                table: "Repositories",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
