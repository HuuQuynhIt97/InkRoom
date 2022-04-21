using INK_API.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace INK_API.Helpers
{
    public class SwaggerOptions
    {
        public string JsonRoute { set; get; }
        public string Description { set; get; }
        public string UIEndpoint { set; get; }
    }
}