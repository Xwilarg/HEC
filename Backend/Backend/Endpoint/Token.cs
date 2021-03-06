﻿using Nancy;
using System.IO;
using System.Web;

namespace Backend.Endpoint
{
    public class Token : NancyModule
    {
        public Token() : base("/token")
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
                return Response.AsJson(new Response.Empty(), HttpStatusCode.NoContent);
            });
        }
    }
}
