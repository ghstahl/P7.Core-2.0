using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace P7.IdentityServer4.Common
{
    // note, only doing this one for consitency.
    // could simply just use global::IdentityServer4.Models.PersistedGrant as is.
    public abstract class AbstractPersistedGrantModel  : IPersistedGrantModel
    {
        public AbstractPersistedGrantModel()
        {
        }

        public AbstractPersistedGrantModel(global::IdentityServer4.Models.PersistedGrant grant)
        {
            ClientId = grant.ClientId;
            CreationTime = grant.CreationTime;
            Data = grant.Data;
            Expiration = grant.Expiration;
            Key = grant.Key;
            SubjectId = grant.SubjectId;
            Type = grant.Type;
        }
        public async Task<PersistedGrant> MakePersistedGrantAsync()
        {
            var result = new global::IdentityServer4.Models.PersistedGrant
            {
                ClientId = ClientId,
                CreationTime = CreationTime,
                Data = Data,
                Expiration = Expiration,
                Key = Key,
                SubjectId = SubjectId,
                Type = Type
            };
            return await Task.FromResult(result);
        }

        public string ClientId { get; set; }
        public DateTime CreationTime { get; set; }
        public string Data { get; set; }
        public DateTime? Expiration { get; set; }
        public string Key { get; set; }
        public string SubjectId { get; set; }
        public string Type { get; set; }
    }
}