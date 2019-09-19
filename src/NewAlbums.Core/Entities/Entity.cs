using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Entities
{
    [Serializable]
    public abstract class Entity<TPrimaryKey>
    {
        public virtual TPrimaryKey Id { get; set; }
    }
}
