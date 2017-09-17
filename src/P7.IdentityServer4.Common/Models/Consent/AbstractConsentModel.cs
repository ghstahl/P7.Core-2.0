using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P7.IdentityServer4.Common
{
    public abstract class AbstractConsentModel<TScopes> : IConsentModel where TScopes : class
    {
        public AbstractConsentModel()
        {
        }

        public AbstractConsentModel(global::IdentityServer4.Models.Consent consent)
        {
            ClientId = consent.ClientId;
            CreationTime = consent.CreationTime;
            Expiration = consent.Expiration;
            SubjectId = consent.SubjectId;
            Scopes = Serialize(consent.Scopes);
        }

        public async Task<global::IdentityServer4.Models.Consent> MakeConsentAsync()
        {
            var result = new global::IdentityServer4.Models.Consent
            {
                ClientId = ClientId,
                Scopes = DeserializeScopes(Scopes),
                SubjectId = SubjectId
            };
            return await Task.FromResult(result);
        }

        private TScopes Serialize(IEnumerable<string> scopes)
        {
            return Serialize(scopes?.ToList());
        }

        public abstract TScopes Serialize(List<string> scopes);

        public abstract List<string> DeserializeScopes(TScopes obj);

        public string ClientId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? Expiration { get; set; }
        public TScopes Scopes { get; set; }
        public string SubjectId { get; set; }

    }
}