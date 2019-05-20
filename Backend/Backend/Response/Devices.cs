using Newtonsoft.Json;
using System.Collections.Generic;

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

        [JsonProperty]
        public Dictionary<string, int> Consumption;
    }

    public class Devices
    {
        [JsonProperty]
        public Device[] AllDevices;
    }
}
