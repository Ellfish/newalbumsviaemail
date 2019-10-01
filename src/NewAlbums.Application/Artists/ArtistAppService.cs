using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewAlbums.Artists.Dto;

namespace NewAlbums.Artists
{
    public class ArtistAppService : BaseAppService, IArtistAppService
    {
        private readonly ICrudServicesAsync _crudServices;

        public ArtistAppService(ICrudServicesAsync crudServices)
        {
            _crudServices = crudServices;
        }

        /// <summary>
        /// When a user subscribes to an artist, we want to store the artist in our database. 
        /// An artist may already exist if another user has already subscribed to them.
        /// When a user subscribes they will usually subscribe to many artists in one go. 
        /// This method is optimised for that scenario, allowing for the fact that some artists may already exist.
        /// </summary>
        public async Task<GetOrCreateManyOutput> GetOrCreateMany(GetOrCreateManyInput input)
        {
            if (!input.Artists.Any())
                throw new ArgumentException("Artists must contain at least one artist", "Artists");

            var spotifyArtistIds = input.Artists.Select(a => a.Id).ToList();

            try
            {
                var output = new GetOrCreateManyOutput();

                var existingArtists = await _crudServices.ReadManyNoTracked<ArtistDto>()
                    .Where(a => spotifyArtistIds.Contains(a.SpotifyId))
                    .ToListAsync();

                foreach (var spotifyArtist in input.Artists)
                {
                    var existingArtist = existingArtists.FirstOrDefault(a => a.SpotifyId == spotifyArtist.Id);
                    if (existingArtist == null)
                    {
                        existingArtist = await _crudServices.CreateAndSaveAsync(new ArtistDto
                        {
                            Name = spotifyArtist.Name,
                            SpotifyId = spotifyArtist.Id
                        });
                    }

                    output.Artists.Add(existingArtist);
                }

                return output;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "SpotifyArtistIds: " + String.Join(',', spotifyArtistIds));
                return new GetOrCreateManyOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<GetAllArtistsOutput> GetAll(GetAllArtistsInput input)
        {
            try
            {
                var allArtists = await _crudServices.ReadManyNoTracked<ArtistDto>()
                    .ToListAsync();

                return new GetAllArtistsOutput
                {
                    Artists = allArtists
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "");
                return new GetAllArtistsOutput
                {
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
