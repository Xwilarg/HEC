using Nancy.Hosting.Self;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System;
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
            R = RethinkDB.R;
            conn = await R.Connection().ConnectAsync();
            LaunchServer(autoEvent);
            autoEvent.WaitOne();
        }

        private void LaunchServer(AutoResetEvent autoEvent)
        {
            HostConfiguration config = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };
            NancyHost host = new NancyHost(config, new Uri("http://localhost:8081"));
            host.Start();
            Console.WriteLine("Host started... Do ^C to exit.");
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("^C received, exitting...");
                host.Dispose();
                autoEvent.Set();
            };
        }

        private RethinkDB R;
        private Connection conn;
    }
}
