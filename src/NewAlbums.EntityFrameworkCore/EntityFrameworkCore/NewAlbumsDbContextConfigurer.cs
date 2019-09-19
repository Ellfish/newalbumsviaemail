using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace NewAlbums.EntityFrameworkCore
{
    public static class NewAlbumsDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<NewAlbumsDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<NewAlbumsDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
