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
    }

    public class Devices
    {
        [JsonProperty]
        public Device[] AllDevices;
    }
}
