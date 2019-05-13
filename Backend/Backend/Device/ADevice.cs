namespace Backend.Device
{
    public abstract class ADevice
    {
        public ADevice(string id, bool isOn)
        {
            _id = id;
            _isOn = isOn;
        }

        private bool _isOn;
        private string _id;
    }
}
