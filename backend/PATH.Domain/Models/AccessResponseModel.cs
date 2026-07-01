using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Domain.Models
{
    public class AccessResponseModel
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
