using Newtonsoft.Json;

namespace Backend.Response
{
    public class Device
    {
        [JsonProperty]
        public bool IsOn;

        [JsonProperty]
        public string Name;

        [JsonProperty]
        public int Power;

        [JsonProperty]
        public string Type;
    }

    public class Devices
    {
        [JsonProperty]
        public Device[] AllDevices;
    }
}
