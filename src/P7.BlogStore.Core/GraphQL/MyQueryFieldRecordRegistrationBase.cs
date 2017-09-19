using System;
using System.Linq;
using GraphQL;
using GraphQL.Types;
using P7.BlogStore.Core.Models;
using P7.GraphQLCore;
using P7.Store;
using P7.SimpleDocument.Store;

namespace P7.BlogStore.Core.GraphQL
{
    public class MyQueryFieldRecordRegistrationBase : IQueryFieldRecordRegistration
    {
        private IBlogStore _blogStore;

        public MyQueryFieldRecordRegistrationBase(
            IBlogStore blogStore)
        {
            _blogStore = blogStore;
        }

        public void AddGraphTypeFields(QueryCore queryCore)
        {
            queryCore.FieldAsync<BlogType>(
                "droid",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> {Name = "id"}),
                resolve: async context =>
                {
                    try
                    {
                        var userContext = context.UserContext.As<GraphQLUserContext>();
                        var id = context.GetArgument<string>("id");
                         var result = await _blogStore.FetchAsync(Guid.Parse(id));
                        return result;
                    }
                    catch (Exception e)
                    {

                    }
                    return null;
                    //                    return await Task.Run(() => { return ""; });
                },
                deprecationReason: null);
            queryCore.FieldAsync<BlogPageType>(
                "droids",
                arguments: new QueryArguments(new QueryArgument<BlogsQueryInput> {Name = "input"}),
                resolve: async context =>
                {
                    try
                    {
                        var userContext = context.UserContext.As<GraphQLUserContext>();
                        var blogsPageHandle = context.GetArgument<BlogsPageHandle>("input");

                        var pagingState = blogsPageHandle.PagingState.SafeConvertFromBase64String();

                        var categories = blogsPageHandle.Categories?.ToArray();
                        var tags = blogsPageHandle.Tags?.ToArray();
                        DateTime baseDateTime = new DateTime();
                        DateTime? timeStampLowerBoundary = baseDateTime == blogsPageHandle.TimeStampLowerBoundary
                            ? (DateTime?) null
                            : blogsPageHandle.TimeStampLowerBoundary;
                        DateTime? timeStampUpperBoundary = baseDateTime == blogsPageHandle.TimeStampUpperBoundary
                            ? (DateTime?) null
                            : blogsPageHandle.TimeStampUpperBoundary;
                        var result = await _blogStore.PageAsync(
                            blogsPageHandle.PageSize,
                            pagingState,
                            timeStampLowerBoundary,
                            timeStampUpperBoundary,
                            categories,
                            tags);

                        var blogPage = new BlogPage()
                        {
                            PagingState =
                                result.PagingState == null ? "" : result.PagingState.SafeConvertToBase64String(),
                            CurrentPagingState =
                                result.CurrentPagingState == null
                                    ? ""
                                    : result.CurrentPagingState.SafeConvertToBase64String(),
                            Blogs = result.ToList()
                        };
                        return blogPage;
                    }
                    catch (Exception e)
                    {

                    }
                    return null;
                    //                    return await Task.Run(() => { return ""; });
                },
                deprecationReason: null);
            queryCore.FieldAsync<BlogDocumentType>(name: "blog",
                description: null,
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<BlogQueryInput>> {Name = "input"}),
                resolve: async context =>
                {
                    try
                    {
                        var userContext = context.UserContext.As<GraphQLUserContext>();
                        var blog = context.GetArgument<SimpleDocument<Blog>>("input");
                        var result = await _blogStore.FetchAsync(blog.Id_G);
                        return result;
                    }
                    catch (Exception e)
                    {

                    }
                    return null;
                    //                    return await Task.Run(() => { return ""; });
                },
                deprecationReason: null);
            queryCore.FieldAsync<BlogPageType>(name: "blogsPage",
                description: null,
                arguments: new QueryArguments(new QueryArgument<BlogsQueryInput> {Name = "input"}),
                resolve: async context =>
                {
                    try
                    {
                        var userContext = context.UserContext.As<GraphQLUserContext>();
                        var blogsPageHandle = context.GetArgument<BlogsPageHandle>("input");

                        var pagingState = blogsPageHandle.PagingState.SafeConvertFromBase64String();

                        var categories = blogsPageHandle.Categories?.ToArray();
                        var tags = blogsPageHandle.Tags?.ToArray();
                        DateTime baseDateTime = new DateTime();
                        DateTime? timeStampLowerBoundary = baseDateTime == blogsPageHandle.TimeStampLowerBoundary
                            ? (DateTime?) null
                            : blogsPageHandle.TimeStampLowerBoundary;
                        DateTime? timeStampUpperBoundary = baseDateTime == blogsPageHandle.TimeStampUpperBoundary
                            ? (DateTime?) null
                            : blogsPageHandle.TimeStampUpperBoundary;
                        var result = await _blogStore.PageAsync(
                            blogsPageHandle.PageSize,
                            pagingState,
                            timeStampLowerBoundary,
                            timeStampUpperBoundary,
                            categories,
                            tags);

                        var blogPage = new BlogPage
                        {
                            CurrentPagingState = result.CurrentPagingState.SafeConvertToBase64String() ?? "",
                            PagingState = result.PagingState.SafeConvertToBase64String() ?? "",
                            Blogs = result.ToList()
                        };
                        return blogPage;
                    }
                    catch (Exception e)
                    {

                    }
                    return null;
                    //                    return await Task.Run(() => { return ""; });
                },
                deprecationReason: null);
            queryCore.FieldAsync<ListGraphType<BlogDocumentType>>(name: "blogsPageByNumber",
                description: null,
                arguments: new QueryArguments(new QueryArgument<BlogsPageQueryInput> {Name = "input"}),
                resolve: async context =>
                {
                    try
                    {
                        var userContext = context.UserContext.As<GraphQLUserContext>();
                        var blogsPageHandle = context.GetArgument<BlogsPageByNumberHandle>("input");

                        var categories = blogsPageHandle.Categories?.ToArray();
                        var tags = blogsPageHandle.Tags?.ToArray();
                        DateTime baseDateTime = new DateTime();
                        DateTime? timeStampLowerBoundary = baseDateTime == blogsPageHandle.TimeStampLowerBoundary
                            ? (DateTime?) null
                            : blogsPageHandle.TimeStampLowerBoundary;
                        DateTime? timeStampUpperBoundary = baseDateTime == blogsPageHandle.TimeStampUpperBoundary
                            ? (DateTime?) null
                            : blogsPageHandle.TimeStampUpperBoundary;
                        var result = await _blogStore.PageAsync(
                            blogsPageHandle.PageSize,
                            blogsPageHandle.Page,
                            timeStampLowerBoundary,
                            timeStampUpperBoundary,
                            categories,
                            tags);
 
                        return result;
                    }
                    catch (Exception e)
                    {

                    }
                    return null;
                    //                    return await Task.Run(() => { return ""; });
                },
                deprecationReason: null);
        }
    }
}
