using Microsoft.EntityFrameworkCore.Migrations;

namespace NewAlbums.Migrations
{
    public partial class Fixed_Typo_In_Subscription_ForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Subscribers_SubscribeId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_SubscribeId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "SubscribeId",
                table: "Subscriptions");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SubscriberId",
                table: "Subscriptions",
                column: "SubscriberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Subscribers_SubscriberId",
                table: "Subscriptions",
                column: "SubscriberId",
                principalTable: "Subscribers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Subscribers_SubscriberId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_SubscriberId",
                table: "Subscriptions");

            migrationBuilder.AddColumn<long>(
                name: "SubscribeId",
                table: "Subscriptions",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SubscribeId",
                table: "Subscriptions",
                column: "SubscribeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Subscribers_SubscribeId",
                table: "Subscriptions",
                column: "SubscribeId",
                principalTable: "Subscribers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
