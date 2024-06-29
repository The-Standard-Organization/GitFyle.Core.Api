using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GitFyle.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class ModifyRepositoryUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Repositories_Name_Owner_ExternalId",
                table: "Repositories");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_Name_Owner_ExternalId_SourceId",
                table: "Repositories",
                columns: new[] { "Name", "Owner", "ExternalId", "SourceId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Repositories_Name_Owner_ExternalId_SourceId",
                table: "Repositories");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_Name_Owner_ExternalId",
                table: "Repositories",
                columns: new[] { "Name", "Owner", "ExternalId" });
        }
    }
}
