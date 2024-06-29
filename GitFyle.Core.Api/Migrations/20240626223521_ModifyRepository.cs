using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GitFyle.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class ModifyRepository : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Repositories_SourceId",
                table: "Repositories");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_Name_Owner_ExternalId",
                table: "Repositories",
                columns: new[] { "Name", "Owner", "ExternalId" });

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_SourceId",
                table: "Repositories",
                column: "SourceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Repositories_Name_Owner_ExternalId",
                table: "Repositories");

            migrationBuilder.DropIndex(
                name: "IX_Repositories_SourceId",
                table: "Repositories");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_SourceId",
                table: "Repositories",
                column: "SourceId");
        }
    }
}
