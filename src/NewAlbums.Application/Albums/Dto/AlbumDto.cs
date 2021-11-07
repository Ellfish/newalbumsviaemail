using System;
using GenericServices;
using NewAlbums.Entities;
using NewAlbums.Spotify.Dto;

namespace NewAlbums.Albums.Dto
{
    public class AlbumDto : CreationAuditedEntityDto<long>, ILinkToEntity<Album>
    {
        public string SpotifyId { get; set; }

        public string Name { get; set; }

        public string ReleaseDate { get; set; }

        /// <summary>
        /// Returns the ReleaseDate as a DateTime if it represents a specific day in format "yyyy-MM-dd", and not just a year or year and month.
        /// Otherwise returns a DateTime initialised to DateTime.MinValue
        /// </summary>
        public DateTime ReleaseDateNormalised
        {
            get
            {
                if (String.IsNullOrWhiteSpace(ReleaseDate) || ReleaseDate.Length != 10)
                    return DateTime.MinValue;

                DateTime releaseDate;
                if (DateTime.TryParse(ReleaseDate, out releaseDate))
                {
                    return releaseDate;
                }

                return DateTime.MinValue;
            }
        }

        public SpotifyImageDto Image { get; set; }

        public AlbumDto()
        {
            //Default
            Image = new SpotifyImageDto();
        }
    }
}
