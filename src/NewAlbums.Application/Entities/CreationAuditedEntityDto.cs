using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Entities
{
    public abstract class CreationAuditedEntityDto<T> : EntityDto<T>
    {
        public DateTime CreatedDate { get; set; }
    }
}
