using Nancy;

namespace Backend.Endpoint
{
    public class Auth : NancyModule
    {
        public Auth() : base("/auth")
        {
            Post("/", x =>
            {
                if (string.IsNullOrEmpty(Request.Query["token"]))
                    return Response.AsJson(new Response.Error()
                    {
                        Message = "Missing arguments"
                    }, HttpStatusCode.BadRequest);
                return new Response.Error() { Message = "a" };
            });
        }
    }
}
