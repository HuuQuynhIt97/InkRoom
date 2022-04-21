using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using INK_API.Models;

namespace INK_API.DTO
{
    public class UserForLoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        
        // public string JwtToken { get; set; }
        // [JsonIgnore]
        // public string RefreshToken { get; set; }
        // public DateTime RefreshTokenExpiration { get; set; }
        
    }
}