using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using P7.Core.Utils;

namespace P7.IdentityServer4.Common.Middleware
{
    public class PublicRefreshTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private IdentityServerOptions _options;
        private readonly ILogger<PublicRefreshTokenMiddleware> _logger;
        private IClientStore _clientStore;
        public PublicRefreshTokenMiddleware(RequestDelegate next,
            ILogger<PublicRefreshTokenMiddleware> logger, 
            IdentityServerOptions options,
            IClientStore clientStore)
        {
            _options = options;
            _next = next;
            _logger = logger;
            _clientStore = clientStore;
        }

        public async Task Invoke(HttpContext context)
        {
            
            if (string.Compare(context.Request.Path, "/connect/token", StringComparison.OrdinalIgnoreCase) == 0)
            {
                var body = await context.Request.ReadFormAsync();
                var id = body["client_id"].FirstOrDefault();
                var refreshToken = body["refresh_token"].FirstOrDefault();
               
                if (id.IsPresent() && refreshToken.IsPresent())
                {
                    if (id.Length <= _options.InputLengthRestrictions.ClientId)
                    {
                        var publicClientId = "public-" + id;
                        var client = await _clientStore.FindClientByIdAsync(publicClientId);
                        if (client != null)
                        {
                            NameValueCollection fcnvc = context.Request.Form.AsNameValueCollection();
                            fcnvc.Set("client_id", publicClientId);
                            fcnvc.Set("grant_type", "public_refresh_token");
                            Dictionary<string, StringValues> dictValues = new Dictionary<string, StringValues>();
                            foreach (var key in fcnvc.AllKeys)
                            {
                                dictValues.Add(key, fcnvc.Get(key));
                            }

                            var fc = new FormCollection(dictValues);
                            context.Request.Form = fc;
                           
                            await PublicRefreshTokenInvokeNext(context);
                            return;
                            
                        }
                    }
                }
            }

             await _next.Invoke(context);
  
        }
       
        private async Task PublicRefreshTokenInvokeNext(HttpContext context)
        {
            using (var buffer = new MemoryStream())
            {
                //replace the context response with our buffer
                var stream = context.Response.Body;
                context.Response.Body = buffer;

                //invoke the rest of the pipeline
                await _next.Invoke(context);
                // we want everything but a 200 to fall through.
                if (context.Response.StatusCode == StatusCodes.Status200OK)
                {
                    //reset the buffer and read out the contents
                    buffer.Seek(0, SeekOrigin.Begin);
                    var reader = new StreamReader(buffer);
                    using (var bufferReader = new StreamReader(buffer))
                    {
                        string body = await bufferReader.ReadToEndAsync();

                        var result = JsonConvert.DeserializeObject<CustomResultDto>(body);
                        var json = ObjectSerializer.ToString(result.inner_response);
                        byte[] arrayOfMyString = Encoding.UTF8.GetBytes(json);
                        using (MemoryStream innerStream = new MemoryStream(arrayOfMyString))
                        {
                            //copy our content to the original stream and put it back
                            await innerStream.CopyToAsync(stream);
                            context.Response.Body = stream;
                        }
                    }
                }
                else
                {
                    //reset to start of stream
                    buffer.Seek(0, SeekOrigin.Begin);

                    //copy our content to the original stream and put it back
                    await buffer.CopyToAsync(stream);
                    context.Response.Body = stream;
                }
            }
        }

        internal class ResultDto
        {
            public string id_token { get; set; }
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
            public string refresh_token { get; set; }
            
        }

        internal class CustomResultDto
        {
            public ResultDto inner_response { get; set; }
        }
    }
}