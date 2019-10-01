using Microsoft.EntityFrameworkCore.Migrations;

namespace NewAlbums.Migrations
{
    public partial class Added_ArtistAlbum_Relationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArtistAlbums",
                columns: table => new
                {
                    ArtistId = table.Column<long>(nullable: false),
                    AlbumId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistAlbums", x => new { x.ArtistId, x.AlbumId });
                    table.ForeignKey(
                        name: "FK_ArtistAlbums_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistAlbums_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistAlbums_AlbumId",
                table: "ArtistAlbums",
                column: "AlbumId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtistAlbums");
        }
    }
}
