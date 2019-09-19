using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NewAlbums.Configuration;
using NewAlbums.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.EntityFrameworkCore
{
    public class NewAlbumsDbContextFactory : IDesignTimeDbContextFactory<NewAlbumsDbContext>
    {
        public NewAlbumsDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<NewAlbumsDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            NewAlbumsDbContextConfigurer.Configure(builder, configuration.GetConnectionString("Default"));

            return new NewAlbumsDbContext(builder.Options);
        }
    }
}
