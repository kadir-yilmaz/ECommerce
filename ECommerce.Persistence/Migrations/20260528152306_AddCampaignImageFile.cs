using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignImageFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteItems_Favorites_FavoriteId",
                table: "FavoriteItems");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteItems_Products_ProductId",
                table: "FavoriteItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_AspNetUsers_UserId",
                table: "Favorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Favorites",
                table: "Favorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteItems",
                table: "FavoriteItems");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteItems_FavoriteId_ProductId",
                table: "FavoriteItems");

            migrationBuilder.RenameTable(
                name: "Favorites",
                newName: "Favorite");

            migrationBuilder.RenameTable(
                name: "FavoriteItems",
                newName: "FavoriteItem");

            migrationBuilder.RenameIndex(
                name: "IX_Favorites_UserId",
                table: "Favorite",
                newName: "IX_Favorite_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteItems_ProductId",
                table: "FavoriteItem",
                newName: "IX_FavoriteItem_ProductId");

            migrationBuilder.AddColumn<bool>(
                name: "ProductImageFile_Showcase",
                table: "Files",
                type: "bit",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Favorite",
                table: "Favorite",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteItem",
                table: "FavoriteItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteItem_FavoriteId",
                table: "FavoriteItem",
                column: "FavoriteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorite_AspNetUsers_UserId",
                table: "Favorite",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteItem_Favorite_FavoriteId",
                table: "FavoriteItem",
                column: "FavoriteId",
                principalTable: "Favorite",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteItem_Products_ProductId",
                table: "FavoriteItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorite_AspNetUsers_UserId",
                table: "Favorite");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteItem_Favorite_FavoriteId",
                table: "FavoriteItem");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteItem_Products_ProductId",
                table: "FavoriteItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteItem",
                table: "FavoriteItem");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteItem_FavoriteId",
                table: "FavoriteItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Favorite",
                table: "Favorite");

            migrationBuilder.DropColumn(
                name: "ProductImageFile_Showcase",
                table: "Files");

            migrationBuilder.RenameTable(
                name: "FavoriteItem",
                newName: "FavoriteItems");

            migrationBuilder.RenameTable(
                name: "Favorite",
                newName: "Favorites");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteItem_ProductId",
                table: "FavoriteItems",
                newName: "IX_FavoriteItems_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Favorite_UserId",
                table: "Favorites",
                newName: "IX_Favorites_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteItems",
                table: "FavoriteItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Favorites",
                table: "Favorites",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteItems_FavoriteId_ProductId",
                table: "FavoriteItems",
                columns: new[] { "FavoriteId", "ProductId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteItems_Favorites_FavoriteId",
                table: "FavoriteItems",
                column: "FavoriteId",
                principalTable: "Favorites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteItems_Products_ProductId",
                table: "FavoriteItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_AspNetUsers_UserId",
                table: "Favorites",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
