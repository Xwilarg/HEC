using Nancy;
using System.IO;
using System.Web;

namespace Backend.Endpoint
{
    public class Switch : NancyModule
    {
        public Switch() : base("/switch")
        {
            Post("/", x =>
            {
                string body;
                using (var reader = new StreamReader(Request.Body)) // x-www-form-urlencoded
                    body = reader.ReadToEnd();
                string token = null, id = null, status = null;
                var parsed = HttpUtility.ParseQueryString(body);
                token = parsed.Get("token");
                id = parsed.Get("id");
                status = parsed.Get("status");
                if (token == null || id == null || status == null)
                    return Response.AsJson(new Response.Error()
                    {
                        Message = "Missing required argument: token"
                    }, HttpStatusCode.BadRequest);
                if (!Program.p.ContainsToken(token))
                    return Response.AsJson(new Response.Error()
                    {
                        Message = "Invalid token, please login again"
                    }, HttpStatusCode.Unauthorized);
                if (status != "false" && status != "true")
                    return Response.AsJson(new Response.Error()
                    {
                        Message = "Status must be true or false"
                    }, HttpStatusCode.BadRequest);
                Device.Device device = Program.p.manager.GetDevice(id);
                if (device == null)
                    return Response.AsJson(new Response.Error()
                    {
                        Message = "Id doesn't match any device"
                    }, HttpStatusCode.BadRequest);
                device.SetStatus(status == "true" ? true : false);
                Program.p.db.UpdateDevice(device);
                Program.p.manager.Update();
                return Response.AsJson(new Response.Empty(), HttpStatusCode.NoContent);
            });
        }
    }
}
