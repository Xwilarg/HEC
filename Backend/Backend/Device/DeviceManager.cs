using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Backend.Device
{
    public class DeviceManager
    {
        public DeviceManager()
        {
            thread = new Thread(new ThreadStart(ConsumptionThread));
            devices = Program.p.db.GetDevices();
        }

        public void Update()
        {
            devices = Program.p.db.GetDevices();
        }

        private void ConsumptionThread()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                Thread.Sleep(60000); // 1 minute
            }
        }

        public bool SetStatus(string id, bool isOn)
        {
            Device d = devices.First(x => x._id == id);
            if (d == null)
                return false;
            d.SetStatus(isOn);
            return true;
        }

        public Device GetDevice(string id)
        {
            Device d = devices.First(x => x._id == id);
            if (d == null)
                return null;
            return d;
        }

        private Thread thread;
        private Device[] devices;
    }
}
