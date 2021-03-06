﻿using System;
using System.Linq;
using System.Threading;

namespace Backend.Device
{
    public class DeviceManager
    {
        public DeviceManager()
        {
            devices = Program.p.db.GetDevices();
            thread = new Thread(new ThreadStart(ConsumptionThread));
            thread.Start();
        }

        private void ConsumptionThread()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                string dateStr = DateTime.Now.ToString("yyMMdd");
                foreach (Device d in devices)
                    d.IncreaseConsumption(dateStr);
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

        public Response.Device[] GetResponseDevices()
        {
            Response.Device[] finalDevices = new Response.Device[devices.Length];
            for (int i = 0; i < devices.Length; i++)
                finalDevices[i] = devices[i].ToResponse();
            return finalDevices;
        }

        private Thread thread;
        private Device[] devices;
    }
}
