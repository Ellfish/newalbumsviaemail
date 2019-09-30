using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewAlbums.Albums.Dto;

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
        /// This method creates albums that don't already exist, and returns only those that were created.
        /// </summary>
        public async Task<CreateNewAlbumsOutput> CreateNewAlbums(CreateNewAlbumsInput input)
        {
            var inputSpotifyAlbumIds = input.Albums.Select(a => a.SpotifyId).ToList();

            try
            {
                var output = new CreateNewAlbumsOutput();

                var existingSpotifyAlbumIds = await _crudServices.ReadManyNoTracked<Album>()
                    .Where(a => inputSpotifyAlbumIds.Contains(a.SpotifyId))
                    .Select(a => a.SpotifyId)
                    .ToListAsync();

                foreach (var inputAlbum in input.Albums)
                {
                    if (!existingSpotifyAlbumIds.Any(id => id == inputAlbum.SpotifyId))
                    {
                        var newAlbum = await _crudServices.CreateAndSaveAsync(new AlbumDto
                        {
                            Name = inputAlbum.Name,
                            SpotifyId = inputAlbum.SpotifyId,
                            ReleaseDate = inputAlbum.ReleaseDate
                        });

                        output.NewAlbums.Add(newAlbum);
                    }
                }

                return output;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "InputSpotifyAlbumIds: " + String.Join(',', inputSpotifyAlbumIds));
                return new CreateNewAlbumsOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
