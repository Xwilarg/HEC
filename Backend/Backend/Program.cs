using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Backend
{
    public class Program
    {
        public static async Task Main(string[] args)
            => await new Program().InitAsync();

        private async Task InitAsync()
        {
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            p = this;
            db = new Db();
            await db.InitAsync();
            tokens = new Dictionary<string, string>();
            rand = new Random();
            manager = new Device.DeviceManager();
            LaunchServer(autoEvent);
            autoEvent.WaitOne();
        }

        private void LaunchServer(AutoResetEvent autoEvent)
        {
            HostConfiguration config = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };
            NancyHost host = new NancyHost(config, new Uri("http://localhost:5151"));
            host.Start();
            Console.WriteLine("Host started... Do ^C to exit.");
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("^C received, exitting...");
                host.Dispose();
                autoEvent.Set();
            };
        }

        public static Program p;
        public Db db { private set; get; }
        public Random rand { private set; get; }

        private Dictionary<string, string> tokens;
        private Device.DeviceManager manager;

        public void AddToken(string username, string token)
        {
            tokens.Add(username, token);
        }

        public bool ContainsToken(string token)
            => tokens.ContainsValue(token);

        public string GetToken(string username)
        {
            if (tokens.ContainsKey(username))
                return tokens[username];
            return null;
        }
    }
}
