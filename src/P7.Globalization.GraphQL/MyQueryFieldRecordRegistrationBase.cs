using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using P7.Core.Localization;
using P7.GraphQLCore;

namespace P7.Globalization
{
    public class MyQueryFieldRecordRegistrationBase : IQueryFieldRecordRegistration
    {
        private IResourceFetcher _resourceFetcher;

        public MyQueryFieldRecordRegistrationBase(
            IResourceFetcher resourceFetcher)
        {
            _resourceFetcher = resourceFetcher;
        }

        public void AddGraphTypeFields(QueryCore queryCore)
        {

            queryCore.FieldAsync<StringGraphType>(name: "resource",
                description: null,
                arguments: new QueryArguments(new QueryArgument<ResourceQueryInput> {Name = "input"}),
                resolve: async context =>
                {
                    var userContext = context.UserContext.As<GraphQLUserContext>();
                    var rqf = userContext.HttpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>();
                    // Culture contains the information of the requested culture
                    CultureInfo currentCulture = rqf.RequestCulture.Culture;

                    var input = context.GetArgument<ResourceQueryHandle>("input");

                    if (!string.IsNullOrEmpty(input.Culture))
                    {
                        try
                        {
                            currentCulture = new CultureInfo(input.Culture);
                        }
                        catch (Exception)
                        {
                            currentCulture = new CultureInfo("en-US");
                        }
                    }
                    var obj = await _resourceFetcher.GetResourceSetAsync(
                        new ResourceQueryHandle()
                        {
                            Culture = currentCulture.Name,
                            Id = input.Id,
                            Treatment = input.Treatment
                        });
                    return obj;
                },
                deprecationReason: null);
        }
    }
}
