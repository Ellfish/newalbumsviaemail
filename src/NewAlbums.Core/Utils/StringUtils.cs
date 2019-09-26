using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Utils
{
    public static class StringUtils
    {
        public static string NormaliseEmailAddress(string emailAddress)
        {
            if (emailAddress == null)
                return null;

            return emailAddress.Trim().ToLower();
        }
    }
}
