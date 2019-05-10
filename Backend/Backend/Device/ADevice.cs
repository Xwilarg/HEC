namespace Backend.Device
{
    public abstract class ADevice
    {
        public ADevice(string id)
        {
            _id = id;
            Program.p.LoadDeviceFromId(id);
        }

        private bool _isOn;
        private string _id;
    }
}
