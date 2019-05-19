using Nancy;
using System.IO;
using System.Web;

namespace Backend.Endpoint
{
    public class Devices : NancyModule
    {
        public Devices() : base("/devices")
        {
            Post("/", x =>
            {
                string body;
                using (var reader = new StreamReader(Request.Body)) // x-www-form-urlencoded
                    body = reader.ReadToEnd();
                string token = null;
                var parsed = HttpUtility.ParseQueryString(body);
                token = parsed.Get("token");
                if (token == null)
                    return Response.AsJson(new Response.Error()
                    {
                        Message = "Missing required argument: token"
                    }, HttpStatusCode.BadRequest);
                if (!Program.p.ContainsToken(token))
                    return Response.AsJson(new Response.Error()
                    {
                        Message = "Invalid token, please login again"
                    }, HttpStatusCode.Unauthorized);
                return Response.AsJson(new Response.Devices()
                {
                    AllDevices = Program.p.manager.GetResponseDevices()
                });
            });
        }
    }
}
