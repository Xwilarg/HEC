using Newtonsoft.Json;

namespace Backend.Response
{
    public class Error
    {
        [JsonProperty]
        public string Message { set; get; }
    }
}
