using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NewAlbums.Entities
{
    public abstract class CreationAuditedEntityDto<T> : EntityDto<T>
    {
        [ReadOnly(true)]
        public DateTime CreatedDate { get; set; }
    }
}
