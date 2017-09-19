using GraphQL.Types;

using P7.SimpleDocument.Store;
using P7.Store;

namespace P7.BlogStore.Core.GraphQL
{
    public class BlogDocumentType : ObjectGraphType<SimpleDocument<Blog>>
    {
        public BlogDocumentType()
        {
            Name = "blogDocument";
            Field(x => x.TenantId).Description("The TenantId of the Blog.");
            Field(x => x.Id).Description("The Id of the Blog.");
            Field<MetaDataType>("metaData", "The MetaData of the Blog.");
            Field<BlogType>("document", "The blog");
        }
    }
    public class BlogType : ObjectGraphType<Blog>
    {
        public BlogType()
        {
            Name = "blog";
            Field(x => x.Title).Description("The Title of the Blog.");
            Field(x => x.Summary).Description("The Summary of the Blog.");
            Field<ListGraphType<StringGraphType>>("categories", "The Categories of the Blog.");
            Field<ListGraphType<StringGraphType>>("tags", "The Tags of the Blog.");
            Field(x => x.TimeStamp).Description("The TimeStamp of the Blog.");
            Field(x => x.Data).Description("The TimeStamp of the Blog.");
        }
    }

}