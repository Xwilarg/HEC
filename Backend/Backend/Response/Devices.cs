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

        [JsonProperty]
        public string RoomName;

        [JsonProperty]
        public string Id;
    }

    public class Devices
    {
        [JsonProperty]
        public Device[] AllDevices;
    }
}
