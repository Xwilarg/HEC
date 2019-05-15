using Newtonsoft.Json;

namespace Backend.Device
{
    public class Device
    {
        public Device(string id, bool isOn, int power, string type, string name)
        {
            _id = id;
            _isOn = isOn;
            _power = power;
            _name = name;
            _type = type;
        }

        public void SetStatus(bool isOn)
        {
            _isOn = isOn;
        }

        public object ToJson()
        {
            return JsonConvert.DeserializeObject("{\"type\":\"" + _type + "\",\"name\":\"" + _name + "\",\"isOn\":" + _isOn + ",\"power\":" + _power + ",\"consumption\":{}}");
        }

        private bool _isOn;
        public string _id { private set; get; }
        private int _power;
        private string _type;
        private string _name;
    }
}
