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
                if (!Program.p.db.AuthentificationCorrect(username, password))
                    return Response.AsJson(new Response.Error()
                    {
                        Message = "Username and password doesn't match"
                    }, HttpStatusCode.Unauthorized);
                string token = Program.p.GetToken(username);
                if (token == null)
                {
                    token = GenerateToken();
                    Program.p.AddToken(username, token);
                }
                return Response.AsJson(new Response.Token()
                {
                    UserToken = token
                });
            });
        }

        private string GenerateToken()
        {
            string characters = "azertyuiopqsdfghjklmwxcvbnAZERTYUIOPQSDFGHJKLMWXCVBN0123456789";
            string token = "";
            for (int i = 0; i < 30; i++)
                token += characters[Program.p.rand.Next(0, characters.Length)];
            return token;
        }
    }
}
