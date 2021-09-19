using SpotifyAPI.Web;

namespace NewAlbums.Spotify.Dto
{
    public class GetSpotifyApiOutput
    {
        public SpotifyClient Api { get; set; }

        public ClientCredentialsTokenResponse TokenResponse { get; set; }
    }
}
