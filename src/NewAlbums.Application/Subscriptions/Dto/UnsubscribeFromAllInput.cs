using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewAlbums.Subscriptions.Dto
{
    public class UnsubscribeFromAllInput
    {
        [Required]
        public string UnsubscribeToken { get; set; }
    }
}
