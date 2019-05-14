using Newtonsoft.Json;

namespace Backend.Response
{
    public class Token
    {
        [JsonProperty]
        public string UserToken { set; get; }
    }
}
