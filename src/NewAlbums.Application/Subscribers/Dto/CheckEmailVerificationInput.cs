using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Subscribers.Dto
{
    public class CheckEmailVerificationInput
    {
        public string EmailAddress { get; set; }

        public string VerifyCode { get; set; }
    }
}
