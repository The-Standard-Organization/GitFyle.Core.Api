using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GitFyle.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedExternalAuditPropertiesForRepositoryContributorContribution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Repositories",
                newName: "ExternalUpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Repositories",
                newName: "ExternalCreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Contributors",
                newName: "ExternalUpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Contributors",
                newName: "ExternalCreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Contributions",
                newName: "ExternalUpdatedAt");

            migrationBuilder.RenameColumn(
                name: "MergedAt",
                table: "Contributions",
                newName: "ExternalMergedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Contributions",
                newName: "ExternalCreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExternalUpdatedAt",
                table: "Repositories",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ExternalCreatedAt",
                table: "Repositories",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ExternalUpdatedAt",
                table: "Contributors",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ExternalCreatedAt",
                table: "Contributors",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ExternalUpdatedAt",
                table: "Contributions",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ExternalMergedAt",
                table: "Contributions",
                newName: "MergedAt");

            migrationBuilder.RenameColumn(
                name: "ExternalCreatedAt",
                table: "Contributions",
                newName: "CreatedAt");
        }
    }
}
