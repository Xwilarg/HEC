using Nancy;
using System.IO;
using System.Web;

namespace Backend.Endpoint
{
    public class Auth : NancyModule
    {
        public Auth() : base("/auth")
        {
            Post("/", x =>
            {
                string body;
                using (var reader = new StreamReader(Request.Body)) // x-www-form-urlencoded
                    body = reader.ReadToEnd();
                string username = null, password = null;
                var parsed = HttpUtility.ParseQueryString(body);
                username = parsed.Get("username");
                password = parsed.Get("password");
                if (username == null || password == null)
                    return Response.AsJson(new Response.Error()
                    {
                        Message = "Missing required arguments: username, password"
                    }, HttpStatusCode.BadRequest);
                if (Program.p.db.AuthentificationCorrect(username, password))
                    return Response.AsJson(new Response.Empty(), HttpStatusCode.NoContent);
                return Response.AsJson(new Response.Error()
                {
                    Message = "Username and password doesn't match"
                }, HttpStatusCode.Unauthorized);
            });
        }
    }
}
