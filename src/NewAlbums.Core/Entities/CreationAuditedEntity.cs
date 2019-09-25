using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Entities
{
    public abstract class CreationAuditedEntity<T> : Entity<T>
    {
        public virtual DateTime CreatedDate { get; set; }

        public CreationAuditedEntity()
        {
            CreatedDate = DateTime.UtcNow;
        }
    }
}
