using Microsoft.EntityFrameworkCore.Migrations;

namespace NewAlbums.Migrations
{
    public partial class Added_EmailAddressVerified_To_Subscriber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailAddressVerified",
                table: "Subscribers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailAddressVerified",
                table: "Subscribers");
        }
    }
}
