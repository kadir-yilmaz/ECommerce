using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCouponRewardSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRewardCoupon",
                table: "DiscountCoupons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxDiscountAmount",
                table: "DiscountCoupons",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "DiscountCoupons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RewardRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PeriodInDays = table.Column<int>(type: "int", nullable: false),
                    MinTotalSpent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RewardCouponAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CouponValidityDays = table.Column<int>(type: "int", nullable: false),
                    CouponMinCartAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserCoupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountCouponId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    UsedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCoupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCoupons_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCoupons_DiscountCoupons_DiscountCouponId",
                        column: x => x.DiscountCouponId,
                        principalTable: "DiscountCoupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupons_DiscountCouponId",
                table: "UserCoupons",
                column: "DiscountCouponId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupons_UserId_DiscountCouponId",
                table: "UserCoupons",
                columns: new[] { "UserId", "DiscountCouponId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RewardRules");

            migrationBuilder.DropTable(
                name: "UserCoupons");

            migrationBuilder.DropColumn(
                name: "IsRewardCoupon",
                table: "DiscountCoupons");

            migrationBuilder.DropColumn(
                name: "MaxDiscountAmount",
                table: "DiscountCoupons");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "DiscountCoupons");
        }
    }
}
