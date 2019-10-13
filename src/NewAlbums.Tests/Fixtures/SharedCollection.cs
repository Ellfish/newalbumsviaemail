using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NewAlbums.Tests.Fixtures
{
    /// <summary>
    /// See: https://xunit.net/docs/shared-context#collection-fixture
    /// </summary>
    [CollectionDefinition("Shared collection")]
    public class SharedCollection : ICollectionFixture<SharedFixture>
    {

    }
}
