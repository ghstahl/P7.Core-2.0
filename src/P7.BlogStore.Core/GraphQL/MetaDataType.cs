using System.Collections.Generic;
using GraphQL.Language.AST;
using GraphQL.Types;
using Newtonsoft.Json;
using P7.SimpleDocument.Store;

namespace P7.BlogStore.Core.GraphQL
{

    public class MetaDataType : ObjectGraphType<MetaData>
    {
        public MetaDataType()
        {
            Name = "metaData";
            Field(x => x.Category).Description("The Category of the blog.");
            Field(x => x.Version).Description("The Version of the blog.");
        }
    }
}