using Newtonsoft.Json;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Backend
{
    public class Db
    {
        public Db()
        {
            R = RethinkDB.R;
        }

        public async Task InitAsync()
        {
            conn = await R.Connection().ConnectAsync();
            if (!await R.DbList().Contains(dbName).RunAsync<bool>(conn))
                await R.DbCreate(dbName).RunAsync(conn);
            if (!await R.Db(dbName).TableList().Contains("Devices").RunAsync<bool>(conn))
            {
                R.Db(dbName).TableCreate("Devices").Run(conn);
                await R.Db(dbName).Table("Devices").Insert(R.HashMap("0", jsonDevice1)
                    .With("1", jsonDevice2)
                    .With("2", jsonDevice3)
                    .With("3", jsonDevice4)
                    ).RunAsync(conn);
            }
            if (!await R.Db(dbName).TableList().Contains("Users").RunAsync<bool>(conn))
            {
                R.Db(dbName).TableCreate("Users").Run(conn);
                // We automatically create admin account from the "Keys/Admins.txt" file is available
                // First line is first username, second line is first password, third line is second username, etc...
                // Passwords must be written in SHA1 in the file
                if (File.Exists("Keys/Admins.txt"))
                {
                    string[] lines = File.ReadAllLines("Keys/Admins.txt");
                    if (lines.Length > 0)
                    {
                        await R.Db(dbName).Table("Users").Insert(R.HashMap(lines[0], lines[1])).RunAsync(conn);
                        for (int i = 2; i < lines.Length; i += 2)
                            await R.Db(dbName).Table("Users").Update(R.HashMap(lines[i], lines[i + 1])).RunAsync(conn);
                    }
                }
            }
        }

        public bool AuthentificationCorrect(string username, string password)
        {
            if (!R.Db(dbName).Table("Users").HasFields(username).IsEmpty().Run<bool>(conn))
            {
                Cursor<string> foundPwd = R.Db(dbName).Table("Users").GetField(username).Run<string>(conn);
                foundPwd.MoveNext();
                if (foundPwd.Current == password)
                    return true;
            }
            return false;
        }

        public Device.Device[] GetDevices()
        {
            List<Device.Device> allDevices = new List<Device.Device>();
            Cursor<object> json = R.Db(dbName).Table("Devices").Run(conn);
            dynamic finalJson = "";
            foreach (dynamic e in json) // Fuck this
            {
                finalJson = e;
                break;
            }
            foreach (dynamic e in finalJson)
            {
                if (e.ToString().StartsWith("\"id\""))
                    continue;
                Match m = Regex.Match(e.ToString(), "\"([^\"]+)\": ({[^}]+})");
                dynamic j2 = JsonConvert.DeserializeObject(m.Groups[2].Value);
                allDevices.Add(new Device.Device(m.Groups[1].Value, j2.isOn, j2.power, j2.type, j2.name));
            }
            return allDevices.ToArray();
        }

        public Response.Device[] GetDevicesResponse()
        {
            List<Response.Device> allDevices = new List<Response.Device>();
            Cursor<object> json = R.Db(dbName).Table("Devices").Run(conn);
            dynamic finalJson = "";
            foreach (dynamic e in json)
            {
                finalJson = e;
                break;
            }
            foreach (dynamic e in finalJson)
            {
                if (e.ToString().StartsWith("\"id\""))
                    continue;
                dynamic j2 = JsonConvert.DeserializeObject(Regex.Match(e.ToString(), "\"[^\"]+\": ({[^}]+})").Groups[1].Value);
                allDevices.Add(new Response.Device()
                {
                    IsOn = j2.isOn,
                    Name = j2.name,
                    Power = j2.power,
                    Type = j2.type
                });
            }
            return allDevices.ToArray();
        }

        public void UpdateDevice(Device.Device device)
        {
            R.Db(dbName).Table("Devices").Update(R.HashMap(device._id, device.ToJson())).Run(conn);
        }

        private RethinkDB R;
        private Connection conn;

        private readonly static string dbName = "HEC";
        // We create sample devices for the demo
        private readonly static object jsonDevice1 = JsonConvert.DeserializeObject("{\"type\":\"lighting\",\"name\":\"Lamp\",\"isOn\":false,\"power\":14,\"consumption\":{}}");
        private readonly static object jsonDevice2 = JsonConvert.DeserializeObject("{\"type\":\"lighting\",\"name\":\"Lamp\",\"isOn\":true,\"power\":20,\"consumption\":{}}");
        private readonly static object jsonDevice3 = JsonConvert.DeserializeObject("{\"type\":\"it\",\"name\":\"Computer\",\"isOn\":true,\"power\":40,\"consumption\":{}}");
        private readonly static object jsonDevice4 = JsonConvert.DeserializeObject("{\"type\":\"heating\",\"name\":\"Heater\",\"isOn\":true,\"power\":30,\"consumption\":{}}");
    }
}
