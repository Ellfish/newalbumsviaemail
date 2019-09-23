using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums
{
    public abstract class BaseOutput
    {
        public string ErrorMessage { get; set; }

        public bool HasError
        {
            get
            {
                return !String.IsNullOrWhiteSpace(ErrorMessage);
            }
        }
    }
}
