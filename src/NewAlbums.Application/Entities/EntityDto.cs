using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NewAlbums.Entities
{
    public abstract class EntityDto<T>
    {
        [ReadOnly(true)]
        public T Id { get; set; }
    }
}
