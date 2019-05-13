using Newtonsoft.Json;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System.IO;
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

        private RethinkDB R;
        private Connection conn;

        private readonly static string dbName = "HEC";
        // We create sample devices for the demo
        private readonly static object jsonDevice1 = JsonConvert.DeserializeObject("{\"name\":\"Lamp\",\"isOn\":false,\"power\":14}");
        private readonly static object jsonDevice2 = JsonConvert.DeserializeObject("{\"name\":\"Lamp\",\"isOn\":true,\"power\":20}");
    }
}
