using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRewardRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RewardCouponAmount",
                table: "RewardRules",
                newName: "RewardDiscountValue");

            migrationBuilder.AddColumn<string>(
                name: "RewardDiscountType",
                table: "RewardRules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "RewardMaxDiscountAmount",
                table: "RewardRules",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RewardDiscountType",
                table: "RewardRules");

            migrationBuilder.DropColumn(
                name: "RewardMaxDiscountAmount",
                table: "RewardRules");

            migrationBuilder.RenameColumn(
                name: "RewardDiscountValue",
                table: "RewardRules",
                newName: "RewardCouponAmount");
        }
    }
}
