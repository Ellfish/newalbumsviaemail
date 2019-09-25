using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Entities
{
    public abstract class EntityDto<T>
    {
        public T Id { get; set; }
    }
}
