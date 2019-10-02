using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewAlbums.Albums.Dto;
using NewAlbums.Artists;

namespace NewAlbums.Albums
{
    public class AlbumAppService : BaseAppService, IAlbumAppService
    {
        private readonly ICrudServicesAsync _crudServices;

        public AlbumAppService(ICrudServicesAsync crudServices)
        {
            _crudServices = crudServices;
        }

        /// <summary>
        /// When we notify a subscriber about an album, we want to store the album in our database so we know
        /// that we've handled the album and don't send more notifications for it in future.
        /// </summary>
        public async Task<CreateAlbumsOutput> CreateAlbums(CreateAlbumsInput input)
        {
            try
            {
                foreach (var inputAlbum in input.Albums)
                {
                    //If the album is attributed to more than one artist, there's a chance the album already exists
                    var existingAlbum = await _crudServices.ReadManyNoTracked<Album>()
                        .Where(a => a.SpotifyId == inputAlbum.SpotifyId)
                        .Include(a => a.Artists)
                        .FirstOrDefaultAsync();

                    if (existingAlbum != null)
                    {
                        //Check if we need to add this artist to the list of artists for the album
                        if (!existingAlbum.Artists.Any(a => a.ArtistId == input.Artist.Id))
                        {
                            Logger.LogInformation("Adding SpotifyArtistId: {0} to existing SpotifyAlbumId: {1} in database", input.Artist.SpotifyId, inputAlbum.SpotifyId);

                            var artistAlbum = new ArtistAlbum
                            {
                                ArtistId = input.Artist.Id,
                                AlbumId = existingAlbum.Id
                            };

                            await _crudServices.CreateAndSaveAsync(artistAlbum);
                        }
                    }
                    else
                    {
                        Logger.LogInformation("Saving SpotifyAlbumId: {0} to database", inputAlbum.SpotifyId);

                        var newAlbum = new Album
                        {
                            Name = inputAlbum.Name,
                            SpotifyId = inputAlbum.SpotifyId,
                            ReleaseDate = inputAlbum.ReleaseDate,
                            Artists = new List<ArtistAlbum>
                            {
                                new ArtistAlbum
                                {
                                    ArtistId = input.Artist.Id
                                }
                            }
                        };

                        await _crudServices.CreateAndSaveAsync(newAlbum);
                    }
                }

                return new CreateAlbumsOutput
                {
                    ErrorMessage = _crudServices.IsValid ? null : _crudServices.GetAllErrors()
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "");
                return new CreateAlbumsOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
