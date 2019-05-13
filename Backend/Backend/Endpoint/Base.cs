using Nancy;

namespace Backend.Endpoint
{
    public class Base : NancyModule
    {
        public Base() : base("/")
        {
            Get("/", x =>
            {
                return Response.AsJson(new Response.Error()
                {
                    Message = "Endpoints: Auth"
                });
            });
        }
    }
}
