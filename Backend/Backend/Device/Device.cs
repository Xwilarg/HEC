using Newtonsoft.Json;
using System.Collections.Generic;

namespace Backend.Device
{
    public class Device
    {
        public Device(string id, bool isOn, int power, string type, string name, string roomName, Dictionary<string, int> consumption)
        {
            _id = id;
            _isOn = isOn;
            _power = power;
            _name = name;
            _type = type;
            _roomName = roomName;
            _consumption = consumption;
        }

        public void SetStatus(bool isOn)
        {
            _isOn = isOn;
        }

        public void IncreaseConsumption(string dateStr)
        {
            if (!_consumption.ContainsKey(dateStr))
                _consumption.Add(dateStr, _power);
            else
                _consumption[dateStr] = _power;
        }

        public object ToJson()
        {
            return JsonConvert.DeserializeObject("{\"roomName\":\"" + _roomName + "\",\"type\":\"" + _type + "\",\"name\":\"" + _name + "\",\"isOn\":" + _isOn.ToString().ToLower() + ",\"power\":" + _power + ",\"consumption\":{}}");
        }

        public Response.Device ToResponse()
        {
            return new Response.Device()
            {
                Id = _id,
                IsOn = _isOn,
                Name = _name,
                Power = _power,
                RoomName = _roomName,
                Type = _type,
                Consumption = _consumption
            };
        }

        private bool _isOn;
        public string _id { private set; get; }
        private int _power;
        private string _type;
        private string _name;
        private string _roomName;
        private Dictionary<string, int> _consumption;
    }
}
