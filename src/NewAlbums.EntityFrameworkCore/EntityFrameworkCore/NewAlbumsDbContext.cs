using Microsoft.EntityFrameworkCore;
using NewAlbums.Artists;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.EntityFrameworkCore
{
    public class NewAlbumsDbContext : DbContext
    {
        public DbSet<Artist> Artists { get; set; }

        public NewAlbumsDbContext(DbContextOptions<NewAlbumsDbContext> options)
            : base(options)
        {

        }
    }
}
