using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using P7.IdentityServer4.AspNetIdentity.Configuration;
using P7.IdentityServer4.Common.Constants;
using P7.IdentityServer4.Common.Models.oidc;
using Converter = P7.IdentityServer4.AspNetIdentity.Configuration.Converter;

namespace P7.IdentityServer4.AspNetIdentity.Stores
{
    public class RemoteIdentityServerDiscoveryStore : IRemoteIdentityServerDiscoveryStore, IIdentityServerTokenStore
    {
        private IOptions<IdentityServerResourceClientCredentials> IdentityServerResourceClientCredentials { get; set; }

        public RemoteIdentityServerDiscoveryStore(
            IOptions<IdentityServerResourceClientCredentials> identityServerResourceClientCredentials)
        {
            IdentityServerResourceClientCredentials = identityServerResourceClientCredentials;
        }

        // "https://localhost:44311/.well-known/openid-configuration";
        public static OpenidConfiguration FromJson(string json) 
            => JsonConvert.DeserializeObject<OpenidConfiguration>(json, Converter.Settings);

        private OpenidConfiguration OpenidConfiguration { get; set; }
        public async Task<OpenidConfiguration> GetRemoteDataAsync(string url)
        {
            var accept = "application/json";
            var uri = url;
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.Accept = accept;
            var content = new MemoryStream();
            OpenidConfiguration record;
            using (WebResponse response = await req.GetResponseAsync())
            {
                using (Stream responseStream = response.GetResponseStream())
                {

                    // Read the bytes in responseStream and copy them to content.
                    await responseStream.CopyToAsync(content);
                    string result = Encoding.UTF8.GetString(content.ToArray());
                    record = FromJson(result);
                }
            }
            return record;
        }

        public async Task<bool> LoadRemoteDataAsync(string url)
        {
            var result = await GetRemoteDataAsync(url);
            if (result != null)
            {
                OpenidConfiguration = result;
                return true;
            }
            return false;
        }

        public OpenidConfiguration GetOpenidConfiguration()
        {
            return OpenidConfiguration;
        }

        public async Task<OIDCRecord> FetchArbitraryResourceOwnerTokens(string clientId, string clientSecret, Dictionary<string, string> arbitraryClaims,
            string userName, string[] arbitraryScopes)
        {
            List<string>  finalQueryList = new List<string>();
            string _s;

            _s = $"client_id={clientId}";
            finalQueryList.Add(_s);

            _s = $"client_secret={clientSecret}";
            finalQueryList.Add(_s);

            _s = "handler=arbitrary_claims_service";
            finalQueryList.Add(_s);

            var _s_arbitraryClaims = JsonConvert.SerializeObject(arbitraryClaims);
            _s = $"{AbritraryOwnerResourceConstants.ArbitraryClaims}={_s_arbitraryClaims}";
            finalQueryList.Add(_s);

            _s = $"username={userName}";
            finalQueryList.Add(_s);

            _s = $"{AbritraryOwnerResourceConstants.ArbitraryScopes}={string.Join(" ", arbitraryScopes)}";
            finalQueryList.Add(_s);

            var _s_finalQuery =  string.Join(" ", finalQueryList);


            throw new System.NotImplementedException();
        }
    }
}