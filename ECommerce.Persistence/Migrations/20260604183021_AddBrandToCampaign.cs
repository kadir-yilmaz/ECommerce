using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandToCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Campaigns",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Campaigns");
        }
    }
}
