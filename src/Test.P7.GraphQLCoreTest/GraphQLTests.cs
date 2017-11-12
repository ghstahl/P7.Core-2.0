using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using FakeItEasy;
using GraphQL;
using GraphQL.Http;
using GraphQL.Language.AST;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQLParser.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using P7.BlogStore.Core;
using P7.BlogStore.Core.GraphQL;
using P7.BlogStore.Core.Models;
using P7.BlogStore.Hugo;
using P7.Core;
using P7.Core.Utils;
using P7.Core.Writers;
using P7.GraphQLCore;
using P7.GraphQLCore.Stores;
using P7.GraphQLCore.Validators;
using P7.HugoStore.Core;
using P7.SimpleDocument.Store;
using P7.Store;
using Shouldly;
namespace Test.P7.GraphQLCoreTest
{
    class MyBlogStoreBiggyConfiguration : IBlogStoreBiggyConfiguration
    {
        public string DatabaseName { get; set; }
        public string FolderStorage { get; set; }
        public string TenantId { get; set; }
    }
    [TestClass]
    [DeploymentItem("source", "source")]
    public class GraphQLTests
    {
        private List<SimpleDocument<Blog>> BlogEntries { get; set; }
        private JsonDocumentWriter _DocumentWriter;

        protected JsonDocumentWriter JsonDocumentWriter
        {
            get
            {
                return _DocumentWriter ?? new JsonDocumentWriter();
            }
        }

        public TenantDatabaseBiggyConfig GlobalTenantDatabaseBiggyConfig { get; set; }
        private string _targetFolder;
        protected string TargetFolder => _targetFolder;
        public IDocumentExecuter Executer { get; private set; }

        public IDocumentWriter Writer { get; private set; }
        public GraphQLUserContext GraphQLUserContext { get; private set; }
        public ISchema Schema
        {
            get
            {
                var schema = AutofacStoreFactory.Resolve<ISchema>();
                return schema;
            }
        }
        private MyAutofacFactory AutofacStoreFactory { get; set; }
        [TestInitialize]
        public async Task Initialize()
        {
            _targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source",
                DateTime.Now.ToString("yyyy-dd-M__HH-mm-ss")+"_"+ Guid.NewGuid().ToString() );
            Directory.CreateDirectory(_targetFolder);
            GlobalTenantDatabaseBiggyConfig = new TenantDatabaseBiggyConfig();
            GlobalTenantDatabaseBiggyConfig.UsingFolder(TargetFolder);
            GlobalTenantDatabaseBiggyConfig.UsingTenantId(TenantDatabaseBiggyConfig.GlobalTenantId);
            IBlogStoreBiggyConfiguration biggyConfiguration = new MyBlogStoreBiggyConfiguration()
            {
                FolderStorage = GlobalTenantDatabaseBiggyConfig.Folder,
                DatabaseName = GlobalTenantDatabaseBiggyConfig.Database,
                TenantId = GlobalTenantDatabaseBiggyConfig.TenantId.ToString()
            };

            var hostName = typeof(MyAutofacFactory).GetTypeInfo().Assembly.GetName().Name;
            var hostingEnvironment = A.Fake<IHostingEnvironment>();
            var httpContextAccessor = A.Fake<IHttpContextAccessor>();


            hostingEnvironment.ApplicationName = hostName;
            Global.HostingEnvironment = hostingEnvironment;
            AutofacStoreFactory = new MyAutofacFactory() { BlogStoreBiggyConfiguration = biggyConfiguration };

            Executer = AutofacStoreFactory.Resolve<IDocumentExecuter>();
            Writer = AutofacStoreFactory.Resolve<IDocumentWriter>();
            GraphQLUserContext = AutofacStoreFactory.Resolve<GraphQLUserContext>();
                       
            var graphQLFieldAuthority = AutofacStoreFactory.Resolve<InMemoryGraphQLFieldAuthority>();
 
            InsertBlogEntriesIntoStore(10);
        }

        [TestMethod]
        public void default_culture_kvo_success_request()
        {
            var d = AutofacStoreFactory.AutofacContainer;

            var dd = AutofacStoreFactory.Resolve<IQueryFieldRecordRegistration>();
            var cc = AutofacStoreFactory.Resolve<IQueryFieldRecordRegistrationStore>();
            var id = @"Test.P7.GraphQLCoreTest.Resources.Main,Test.P7.GraphQLCoreTest";
            var treatment = @"kvo";
            var gqlInputs = $"{{'input': {{'id':'{id}', 'treatment': '{treatment}' }} }}"
                .ToInputs();
            var query = @"
                query Q($input: ResourceQueryInput!) {
                  resource(input: $input)
                }";

            var expected = @"{'resource':{'hello':'Hello'}}";
            AssertQuerySuccess(query, expected, gqlInputs,root:null,userContext: GraphQLUserContext);


            Assert.AreEqual(42, 42);
        }
        [TestMethod]
        public void default_culture_kva_success_request()
        {
            var d = AutofacStoreFactory.AutofacContainer;

            var dd = AutofacStoreFactory.Resolve<IQueryFieldRecordRegistration>();
            var cc = AutofacStoreFactory.Resolve<IQueryFieldRecordRegistrationStore>();
            var id = @"Test.P7.GraphQLCoreTest.Resources.Main,Test.P7.GraphQLCoreTest";
            var treatment = @"kva";
            var gqlInputs = $"{{'input': {{'id':'{id}', 'treatment': '{treatment}' }} }}"
                .ToInputs();
            var query = @"
                query Q($input: ResourceQueryInput!) {
                  resource(input: $input)
                }";


            var expected = @"{'resource':[{'key': 'Hello','value': 'Hello'}]}";
            AssertQuerySuccess(query, expected, gqlInputs, root: null, userContext: GraphQLUserContext);



            Assert.AreEqual(42, 42);
        }

        List<SimpleDocument<Blog>> CreateBlogEntries(int nCount)
        {
            if (this.BlogEntries == null)
            {
                this.BlogEntries = new EditableList<SimpleDocument<Blog>>();

                var dtBase = DateTime.UtcNow.ToSecondResolution();
                dtBase = dtBase.AddYears(10);
                var tsS = dtBase.ToString(JsonDocumentWriter.JsonSerializerSettings.DateFormatString);

                dtBase = DateTime.Parse(tsS, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);



                int timeMultipier = 1;
                for (int i = 0; i < 10; ++i)
                {
                    timeMultipier = timeMultipier * -1;
                    DateTime timeStamp = dtBase.AddMinutes(i * timeMultipier);
                    int n = i % 2;
                    SimpleDocument<Blog> simpleBlogDocument = new SimpleDocument<Blog>()
                    {
                        MetaData = new MetaData() {Category = "c0", Version = "1.0.0.0"},
                        Document = new Blog()
                        {
                            Categories = new List<string>() {"c1" + n, "c2" + i},
                            Tags = new List<string>() {"t1" + n, "t2" + i},
                            Data = "This is my blog",
                            TimeStamp = timeStamp,
                            Summary = "My Summary",
                            Title = "My Title"
                        },
                        Id = Guid.NewGuid().ToString()
                    };

                    
                    BlogEntries.Add(simpleBlogDocument);
                }
            }
            return BlogEntries;
        }

        void InsertBlogEntriesIntoStore(int count)
        {
            if (!Inserted)
            {
                CreateBlogEntries(count);
                var pluginValidationRules = AutofacStoreFactory.Resolve<IEnumerable<IPluginValidationRule>>();

                foreach (var blogEntry in BlogEntries)
                {
                    var jsonBlog = JsonDocumentWriter.SerializeObjectSingleQuote(blogEntry);
                    Dictionary<string, object> values =
                        JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonBlog);
                    var rawInput = $"{{'input': {jsonBlog} }}";
                    var gqlInputs = rawInput.ToInputs();
                    var mutation = @"  mutation Q($input: blogMutationInput!) {
                      blog(input: $input)
                    }";

                    var expected = @"{'blog':true}";
                    AssertQuerySuccess(mutation, expected, gqlInputs, root: null, userContext: GraphQLUserContext, rules: pluginValidationRules);
                    rawInput = $"{{'input': {{ 'id':'{blogEntry.Id}' }} }}";
                    gqlInputs = rawInput.ToInputs();
                    var query = @"query Q($input: blogQueryInput!) {
                      blog(input: $input){
                            tenantId
                            id
                            metaData {
                                category
                                version
                            }
                            document{
                                title
                                summary
                                categories
                                tags
                                timeStamp
                                data
                            }
                        }
                    }";
                  

                    var runResult = ExecuteQuery(query, gqlInputs, root: null, userContext: GraphQLUserContext,rules: pluginValidationRules);
                    bool bErrors = runResult.Errors?.Any() == true;
                    Assert.IsFalse(bErrors);

                    Dictionary<string, object> data = (Dictionary<string, object>) runResult.Data;
                    var resultJson = JsonConvert.SerializeObject(data["blog"]);
                    SimpleDocument<Blog> resultBlogDocument =
                        JsonConvert.DeserializeObject<SimpleDocument<Blog>>(resultJson);
                    resultBlogDocument.TenantId = blogEntry.TenantId;
                    resultBlogDocument.Document.EnableDeepCompare = blogEntry.Document.EnableDeepCompare;
                    string additionalInfo = null;
                    blogEntry.Document.EnableDeepCompare = true;
                    resultBlogDocument.ShouldBe(blogEntry, additionalInfo);
 
                 }
                BlogEntries = BlogEntries.OrderBy(o => o.Document.TimeStamp).ToList();
                foreach (var item in BlogEntries)
                {
                    item.Document.EnableDeepCompare = true;
                }

            }
            Inserted = true;
        }

        public bool Inserted { get; set; }

        [TestMethod]
        public void blog_timestamp_lower_and_upper_boundary_paging_request_by_number()
        {

            var timestampLowerBoundary = BlogEntries[3].Document.TimeStamp;
            var timestampUpperBoundary = BlogEntries[4].Document.TimeStamp;
            var tsLowerAsString = timestampLowerBoundary.ToString("u");
            var tsUpperAsString = timestampUpperBoundary.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            // ASK FOR THEM ALL
            var pageSize = BlogEntries.Count;
            var rawInput2 = $"{{'input': {{'pageSize':'{pageSize}','page':1,'timestampLowerBoundary':'{tsLowerAsString}','timestampUpperBoundary':'{tsUpperAsString}' }} }}";
            var gqlInputs2 = rawInput2.ToInputs();
            var query2 = @"query Q($input: blogsPageQueryInput!) {
                  blogsPageByNumber(input: $input){
                    tenantId
                    id
                    metaData {
                        category
                        version
                    }
                    document{
                        title
                        summary
                        categories
                        tags
                        timeStamp
                        data
                    }
                  }
                }";
            var runResult2 = ExecuteQuery(query2, gqlInputs2, root: null, userContext: GraphQLUserContext);
            var bRun2 = runResult2.Errors?.Any() == true;
            Assert.IsFalse(bRun2);

            var slice = BlogEntries.Skip(3).Take(2).ToList();
            foreach (var item in slice)
            {
                item.Document.EnableDeepCompare = true;
            }
            Dictionary<string, object> result = (Dictionary<string, object>)runResult2.Data;
            var resultJson = JsonConvert.SerializeObject(result["blogsPageByNumber"]);
            var blogs = JsonConvert.DeserializeObject<List<SimpleDocument<Blog>>>(resultJson);

            var read = blogs;
            slice.ShouldBe(read);
            blogs.Count.ShouldBe(slice.Count);

        }

        [TestMethod]
        public void blog_timestamp_lower_and_upper_boundary_paging_request()
        {

            var timestampLowerBoundary = BlogEntries[3].Document.TimeStamp;
            var timestampUpperBoundary = BlogEntries[4].Document.TimeStamp;
            var tsLowerAsString = timestampLowerBoundary.ToString("u");
            var tsUpperAsString = timestampUpperBoundary.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            // ASK FOR THEM ALL
            var pageSize = BlogEntries.Count;
            var rawInput2 = $"{{'input': {{'pageSize':'{pageSize}','timestampLowerBoundary':'{tsLowerAsString}','timestampUpperBoundary':'{tsUpperAsString}' }} }}";
            var gqlInputs2 = rawInput2.ToInputs();
            var query2 = @"query Q($input: blogsQueryInput!) {
                  blogsPage(input: $input){
                    currentPagingState
                    pagingState
                    blogs {
                        tenantId
                        id
                        metaData {
                            category
                            version
                        }
                        document{
                            title
                            summary
                            categories
                            tags
                            timeStamp
                            data
                        }
                    }
                  }
                }";
            var runResult2 = ExecuteQuery(query2, gqlInputs2, root: null, userContext: GraphQLUserContext);
            var bRun2 = runResult2.Errors?.Any() == true;
            Assert.IsFalse(bRun2);

            var slice = BlogEntries.Skip(3).Take(2).ToList();
            foreach (var item in slice)
            {
                item.Document.EnableDeepCompare = true;
            }
            Dictionary<string, object> result = (Dictionary<string, object>)runResult2.Data;
            var resultJson = JsonConvert.SerializeObject(result["blogsPage"]);
            var blogPage = JsonConvert.DeserializeObject<BlogPage>(resultJson);

            var read = blogPage.Blogs;
            slice.ShouldBe(read);
            blogPage.Blogs.Count.ShouldBe(slice.Count);

        }

        [TestMethod]
        public void blog_timestamp_categories_lower_and_upper_boundary_paging_request_by_number()
        {

            var timestampLowerBoundary = BlogEntries[0].Document.TimeStamp;
            var timestampUpperBoundary = BlogEntries[9].Document.TimeStamp;
            var categories = BlogEntries[2].Document.Categories;
            var jsonCategories = JsonConvert.SerializeObject(categories);
            var tsLowerAsString = timestampLowerBoundary.ToString("u");
            var tsUpperAsString = timestampUpperBoundary.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            // ASK FOR THEM ALL
            var pageSize = BlogEntries.Count;
            var rawInput2 = $"{{'input': {{'pageSize':'{pageSize}','page':1,'timestampLowerBoundary':'{tsLowerAsString}','timestampUpperBoundary':'{tsUpperAsString}','categories':{jsonCategories} }} }}";
            var gqlInputs2 = rawInput2.ToInputs();
            var query2 = @"query Q($input: blogsPageQueryInput!) {
                  blogsPageByNumber(input: $input){
                    tenantId
                    id
                    metaData {
                        category
                        version
                    }
                    document{
                        title
                        summary
                        categories
                        tags
                        timeStamp
                        data
                    }
                  }
                }";
            var runResult2 = ExecuteQuery(query2, gqlInputs2, root: null, userContext: GraphQLUserContext);
            var bRun2 = runResult2.Errors?.Any() == true;
            Assert.IsFalse(bRun2);
            var query = from item in BlogEntries
                where item.Document.Categories.Any(a => categories.Contains(a))
                select item;
            var slice = query.ToList();

            Dictionary<string, object> result = (Dictionary<string, object>)runResult2.Data;
            var resultJson = JsonConvert.SerializeObject(result["blogsPageByNumber"]);
            var blogs = JsonConvert.DeserializeObject<List<SimpleDocument<Blog>>>(resultJson);

            var read = blogs;
            slice.ShouldBe(read);
            blogs.Count.ShouldBe(slice.Count);

        }

        [TestMethod]
        public void blog_timestamp_categories_lower_and_upper_boundary_paging_request()
        {

            var timestampLowerBoundary = BlogEntries[0].Document.TimeStamp;
            var timestampUpperBoundary = BlogEntries[9].Document.TimeStamp;
            var categories = BlogEntries[2].Document.Categories;
            var jsonCategories = JsonConvert.SerializeObject(categories);
            var tsLowerAsString = timestampLowerBoundary.ToString("u");
            var tsUpperAsString = timestampUpperBoundary.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            // ASK FOR THEM ALL
            var pageSize = BlogEntries.Count;
            var rawInput2 = $"{{'input': {{'pageSize':'{pageSize}','timestampLowerBoundary':'{tsLowerAsString}','timestampUpperBoundary':'{tsUpperAsString}','categories':{jsonCategories} }} }}";
            var gqlInputs2 = rawInput2.ToInputs();
            var query2 = @"query Q($input: blogsQueryInput!) {
                  blogsPage(input: $input){
                    currentPagingState
                    pagingState
                    blogs {
                        tenantId
                        id
                        metaData {
                            category
                            version
                        }
                        document{
                            title
                            summary
                            categories
                            tags
                            timeStamp
                            data
                        }
                    }
                  }
                }";
            var runResult2 = ExecuteQuery(query2, gqlInputs2, root: null, userContext: GraphQLUserContext);
            var bRun2 = runResult2.Errors?.Any() == true;
            Assert.IsFalse(bRun2);
            var query = from item in BlogEntries
                        where item.Document.Categories.Any(a => categories.Contains(a))
                        select item;
            var slice = query.ToList();

            Dictionary<string, object> result = (Dictionary<string, object>)runResult2.Data;
            var resultJson = JsonConvert.SerializeObject(result["blogsPage"]);
            var blogPage = JsonConvert.DeserializeObject<BlogPage>(resultJson);

            var read = blogPage.Blogs;
            slice.ShouldBe(read);
            blogPage.Blogs.Count.ShouldBe(slice.Count);

        }

        [TestMethod]
        public void blog_timestamp_tags_lower_and_upper_boundary_paging_request_by_number()
        {

            var timestampLowerBoundary = BlogEntries[0].Document.TimeStamp;
            var timestampUpperBoundary = BlogEntries[9].Document.TimeStamp;
            var tags = BlogEntries[2].Document.Tags;
            var jsonTags = JsonConvert.SerializeObject(tags);
            var tsLowerAsString = timestampLowerBoundary.ToString("u");
            var tsUpperAsString = timestampUpperBoundary.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            // ASK FOR THEM ALL
            var pageSize = BlogEntries.Count;
            var rawInput2 = $"{{'input': {{'pageSize':'{pageSize}','page':1,'timestampLowerBoundary':'{tsLowerAsString}','timestampUpperBoundary':'{tsUpperAsString}','tags':{jsonTags} }} }}";
            var gqlInputs2 = rawInput2.ToInputs();
            var query2 = @"query Q($input: blogsPageQueryInput!) {
                  blogsPageByNumber(input: $input){
                    tenantId
                    id
                    metaData {
                        category
                        version
                    }
                    document{
                        title
                        summary
                        categories
                        tags
                        timeStamp
                        data
                    }
                  }
                }";
            var runResult2 = ExecuteQuery(query2, gqlInputs2, root: null, userContext: GraphQLUserContext);
            var bRun2 = runResult2.Errors?.Any() == true;
            Assert.IsFalse(bRun2);
            var query = from item in BlogEntries
                        where item.Document.Tags.Any(a => tags.Contains(a))
                        select item;
            var slice = query.ToList();

            Dictionary<string, object> result = (Dictionary<string, object>)runResult2.Data;
            var resultJson = JsonConvert.SerializeObject(result["blogsPageByNumber"]);
            var blogs = JsonConvert.DeserializeObject<List<SimpleDocument<Blog>>>(resultJson);

            var read = blogs;
            slice.ShouldBe(read);
            blogs.Count.ShouldBe(slice.Count);

        }

        [TestMethod]
        public void blog_timestamp_tags_lower_and_upper_boundary_paging_request()
        {

            var timestampLowerBoundary = BlogEntries[0].Document.TimeStamp;
            var timestampUpperBoundary = BlogEntries[9].Document.TimeStamp;
            var tags = BlogEntries[2].Document.Tags;
            var jsonTags = JsonConvert.SerializeObject(tags);
            var tsLowerAsString = timestampLowerBoundary.ToString("u");
            var tsUpperAsString = timestampUpperBoundary.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            // ASK FOR THEM ALL
            var pageSize = BlogEntries.Count;
            var rawInput2 = $"{{'input': {{'pageSize':'{pageSize}','timestampLowerBoundary':'{tsLowerAsString}','timestampUpperBoundary':'{tsUpperAsString}','tags':{jsonTags} }} }}";
            var gqlInputs2 = rawInput2.ToInputs();
            var query2 = @"query Q($input: blogsQueryInput!) {
                  blogsPage(input: $input){
                    currentPagingState
                    pagingState
                    blogs {
                        tenantId
                        id
                        metaData {
                            category
                            version
                        }
                        document{
                            title
                            summary
                            categories
                            tags
                            timeStamp
                            data
                        }
                    }
                  }
                }";
            var runResult2 = ExecuteQuery(query2, gqlInputs2, root: null, userContext: GraphQLUserContext);
            var bRun2 = runResult2.Errors?.Any() == true;
            Assert.IsFalse(bRun2);
            var query = from item in BlogEntries
                        where item.Document.Tags.Any(a => tags.Contains(a))
                        select item;
            var slice = query.ToList();

            Dictionary<string, object> result = (Dictionary<string, object>)runResult2.Data;
            var resultJson = JsonConvert.SerializeObject(result["blogsPage"]);
            var blogPage = JsonConvert.DeserializeObject<BlogPage>(resultJson);

            var read = blogPage.Blogs;
            slice.ShouldBe(read);
            blogPage.Blogs.Count.ShouldBe(slice.Count);

        }

        [TestMethod]
        public void blog_timestamp_lower_boundary_paging_request_by_number()
        {
            BlogEntries.Count.ShouldBe(10);
            var timestampLowerBoundary = BlogEntries[3].Document.TimeStamp;
            var tsAsString = timestampLowerBoundary.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            // ASK FOR THEM ALL
            var pageSize = BlogEntries.Count;
            var rawInput2 = $"{{'input': {{'pageSize':'{pageSize}','page':1,'timestampLowerBoundary':'{tsAsString}' }} }}";
            var gqlInputs2 = rawInput2.ToInputs();
            var query2 = @"query Q($input: blogsPageQueryInput!) {
                  blogsPageByNumber(input: $input){
                    tenantId
                    id
                    metaData {
                        category
                        version
                    }
                    document{
                        title
                        summary
                        categories
                        tags
                        timeStamp
                        data
                    }
                  }
                }";
            var runResult2 = ExecuteQuery(query2, gqlInputs2, root: null, userContext: GraphQLUserContext);
            var bRun2 = runResult2.Errors?.Any() == true;
            Assert.IsFalse(bRun2);

            var slice = BlogEntries.Skip(3).Take(pageSize).ToList();
            foreach (var item in slice)
            {
                item.Document.EnableDeepCompare = true;
            }
            Dictionary<string, object> result = (Dictionary<string, object>)runResult2.Data;
            var resultJson = JsonConvert.SerializeObject(result["blogsPageByNumber"]);
            var blogs = JsonConvert.DeserializeObject<List<SimpleDocument<Blog>>>(resultJson);

            var read = blogs;
            slice.ShouldBe(read);
            blogs.Count.ShouldBe(slice.Count);

        }

        [TestMethod]
        public void blog_timestamp_lower_boundary_paging_request()
        {
            BlogEntries.Count.ShouldBe(10);
            var timestampLowerBoundary = BlogEntries[3].Document.TimeStamp;
            var tsAsString = timestampLowerBoundary.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            // ASK FOR THEM ALL
            var pageSize = BlogEntries.Count;
            var rawInput2 = $"{{'input': {{'pageSize':'{pageSize}','timestampLowerBoundary':'{tsAsString}' }} }}";
            var gqlInputs2 = rawInput2.ToInputs();
            var query2 = @"query Q($input: blogsQueryInput!) {
                  blogsPage(input: $input){
                    currentPagingState
                    pagingState
                    blogs {
                        tenantId
                        id
                        metaData {
                            category
                            version
                        }
                        document{
                            title
                            summary
                            categories
                            tags
                            timeStamp
                            data
                        }
                    }
                  }
                }";
            var runResult2 = ExecuteQuery(query2, gqlInputs2, root: null, userContext: GraphQLUserContext);
            var bRun2 = runResult2.Errors?.Any() == true;
            Assert.IsFalse(bRun2);

            var slice = BlogEntries.Skip(3).Take(pageSize).ToList();
            foreach (var item in slice)
            {
                item.Document.EnableDeepCompare = true;
            }
            Dictionary<string, object> result = (Dictionary<string, object>)runResult2.Data;
            var resultJson = JsonConvert.SerializeObject(result["blogsPage"]);
            var blogPage = JsonConvert.DeserializeObject<BlogPage>(resultJson);

            var read = blogPage.Blogs;
            slice.ShouldBe(read);
            blogPage.Blogs.Count.ShouldBe(slice.Count);

        }

        [TestMethod]
        public void blog_timestamp_upper_boundary_paging_request_by_number()
        {
            BlogEntries.Count.ShouldBe(10);

            var timestampUpperBoundary = BlogEntries[3].Document.TimeStamp;
            var tsAsString = timestampUpperBoundary.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            // ASK FOR THEM ALL
            var pageSize = BlogEntries.Count;
            var rawInput2 = $"{{'input': {{'pageSize':'{pageSize}','page':1,'timestampUpperBoundary':'{tsAsString}' }} }}";
            var gqlInputs2 = rawInput2.ToInputs();
            var query2 = @"query Q($input: blogsPageQueryInput!) {
                  blogsPageByNumber(input: $input){
                    tenantId
                    id
                    metaData {
                        category
                        version
                    }
                    document{
                        title
                        summary
                        categories
                        tags
                        timeStamp
                        data
                    }
                  }
                }";
            var runResult2 = ExecuteQuery(query2, gqlInputs2, root: null, userContext: GraphQLUserContext);
            var bRun2 = runResult2.Errors?.Any() == true;
            Assert.IsFalse(bRun2);

            var slice = BlogEntries.Skip(0).Take(4).ToList();
            foreach (var item in slice)
            {
                item.Document.EnableDeepCompare = true;
            }
            Dictionary<string, object> result = (Dictionary<string, object>)runResult2.Data;
            var resultJson = JsonConvert.SerializeObject(result["blogsPageByNumber"]);
            var blogs = JsonConvert.DeserializeObject<List<SimpleDocument<Blog>>>(resultJson);

            var read = blogs;
            slice.ShouldBe(read);
            blogs.Count.ShouldBe(slice.Count);

        }
        [TestMethod]
        public void blog_timestamp_upper_boundary_paging_request()
        {
            BlogEntries.Count.ShouldBe(10);

            var timestampUpperBoundary = BlogEntries[3].Document.TimeStamp;
            var tsAsString = timestampUpperBoundary.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            // ASK FOR THEM ALL
            var pageSize = BlogEntries.Count;
            var rawInput2 = $"{{'input': {{'pageSize':'{pageSize}','timestampUpperBoundary':'{tsAsString}' }} }}";
            var gqlInputs2 = rawInput2.ToInputs();
            var query2 = @"query Q($input: blogsQueryInput!) {
                  blogsPage(input: $input){
                    currentPagingState
                    pagingState
                    blogs {
                        tenantId
                        id
                        metaData {
                            category
                            version
                        }
                        document{
                            title
                            summary
                            categories
                            tags
                            timeStamp
                            data
                        }
                    }
                  }
                }";
            var runResult2 = ExecuteQuery(query2, gqlInputs2, root: null, userContext: GraphQLUserContext);
            var bRun2 = runResult2.Errors?.Any() == true;
            Assert.IsFalse(bRun2);

            var slice = BlogEntries.Skip(0).Take(4).ToList();
            foreach (var item in slice)
            {
                item.Document.EnableDeepCompare = true;
            }
            Dictionary<string, object> result = (Dictionary<string, object>)runResult2.Data;
            var resultJson = JsonConvert.SerializeObject(result["blogsPage"]);
            var blogPage = JsonConvert.DeserializeObject<BlogPage>(resultJson);

            var read = blogPage.Blogs;
            slice.ShouldBe(read);
            blogPage.Blogs.Count.ShouldBe(slice.Count);

        }

        [TestMethod]
        public void blog_basic_paging_request_by_number()
        {

            BlogEntries.Count.ShouldBe(10);

            var pageSize = 3;
            var currentIndex = 0;
            var count = 0;
            int page = 1;
            bool keepGoing = true;
            do
            {
                var rawInput2 = $"{{'input': {{'pageSize':{pageSize},'page':{page} }} }}";
                var gqlInputs2 = rawInput2.ToInputs();
                var query2 = @"query Q($input: blogsPageQueryInput!) {
                  blogsPageByNumber(input: $input){
                    tenantId
                    id
                    metaData {
                        category
                        version
                    }
                    document{
                        title
                        summary
                        categories
                        tags
                        timeStamp
                        data
                    }
                  }
                }";
                var runResult2 = ExecuteQuery(query2, gqlInputs2, root: null, userContext: GraphQLUserContext);
                bool bRun2 = runResult2.Errors?.Any() == true;
                Assert.IsFalse(bRun2);


                var slice = BlogEntries.Skip(currentIndex).Take(pageSize).ToList();
                foreach (var item in slice)
                {
                    item.Document.EnableDeepCompare = true;
                }
                Dictionary<string, object> result = (Dictionary<string, object>)runResult2.Data;
                var resultJson = JsonConvert.SerializeObject(result["blogsPageByNumber"]);
                var blogs = JsonConvert.DeserializeObject<List<SimpleDocument<Blog>>>(resultJson);

                var read = blogs;
                slice.ShouldBe(read);
                blogs.Count.ShouldBe(slice.Count);

                count += read.Count;
                currentIndex += pageSize;
                keepGoing = blogs.Count != 0;

                page = page + 1;
            } while (keepGoing);

            System.Diagnostics.Debug.WriteLine("Count:"+count);
            count.ShouldBe(BlogEntries.Count);
        }

        [TestMethod]
        public void blog_basic_paging_request()
        {

            BlogEntries.Count.ShouldBe(10);

            var pageSize = 3;
            var currentIndex = 0;
            var count = 0;
            byte[] pagingState = null;
            byte[] currentPagingState = null;
            bool keepGoing = true;
            do
            {
                var rawInput2 = $"{{'input': {{'pageSize':'{pageSize}','pagingState':'{pagingState.SafeConvertToBase64String()}' }} }}";
                var gqlInputs2 = rawInput2.ToInputs();
                var query2 = @"query Q($input: blogsQueryInput!) {
                  blogsPage(input: $input){
                    currentPagingState
                    pagingState
                    blogs {
                        tenantId
                        id
                        metaData {
                            category
                            version
                        }
                        document{
                            title
                            summary
                            categories
                            tags
                            timeStamp
                            data
                        }
                    }
                  }
                }";
                var runResult2 = ExecuteQuery(query2, gqlInputs2, root: null, userContext: GraphQLUserContext);
                bool bRun2 = runResult2.Errors?.Any() == true;
                Assert.IsFalse(bRun2);


                var slice = BlogEntries.Skip(currentIndex).Take(pageSize).ToList();
                foreach (var item in slice)
                {
                    item.Document.EnableDeepCompare = true;
                }
                Dictionary<string, object> result = (Dictionary<string, object>)runResult2.Data;
                var resultJson = JsonConvert.SerializeObject(result["blogsPage"]);
                var blogPage = JsonConvert.DeserializeObject<BlogPage>(resultJson);

                var read = blogPage.Blogs;

                slice.ShouldBe(read);
                pagingState = blogPage.PagingState.SafeConvertFromBase64String();
                currentPagingState = blogPage.CurrentPagingState.SafeConvertFromBase64String();

                count += read.Count;
                currentIndex += pageSize;
                keepGoing = pagingState != null;

            } while (keepGoing);

            System.Diagnostics.Debug.WriteLine("Count:" + count);
            count.ShouldBe(BlogEntries.Count);
        }
        [TestMethod]
        public void blog_add_query_request()
        {
            var d = AutofacStoreFactory.AutofacContainer;

            var dd = AutofacStoreFactory.Resolve<IMutationFieldRecordRegistration>();
            var cc = AutofacStoreFactory.Resolve<IMutationFieldRecordRegistrationStore>();
            var timeStamp = DateTime.UtcNow.ToSecondResolution();
            var tsS = timeStamp.ToString(JsonDocumentWriter.JsonSerializerSettings.DateFormatString);

            var simpleTS = DateTime.Parse(tsS, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);

            var blogEntry = new SimpleDocument<Blog>()
            {
                MetaData = new MetaData() { Category = "c0", Version = "1.0.0.0" },
                Document = new Blog()
                {
                    Categories = new List<string>() { "c1", "c2" },
                    Tags = new List<string>() { "t1", "t2" },
                    Data = "This is my blog",
                    TimeStamp = timeStamp,
                    Summary = "My Summary",
                    Title = "My Title"
                },
                Id = Guid.NewGuid().ToString()
            };
             
            var jsonBlog = JsonDocumentWriter.SerializeObjectSingleQuote(blogEntry);

            var rawInput = $"{{'input': {jsonBlog} }}";

            var gqlInputs = rawInput.ToInputs();
            var mutation = @"
                mutation Q($input: blogMutationInput!) {
                  blog(input: $input)
                }";

            var expected = @"{'blog':true}";
            AssertQuerySuccess(mutation, expected, gqlInputs, root: null, userContext: GraphQLUserContext);

            rawInput =
               $"{{'input': {{'id':'{blogEntry.Id.ToString()}' }} }}";
            gqlInputs = rawInput.ToInputs();
            var query = @"query Q($input: blogQueryInput!) {
                      blog(input: $input){
                            tenantId
                            id
                            metaData {
                                category
                                version
                            }
                            document{
                                title
                                summary
                                categories
                                tags
                                timeStamp
                                data
                            }
                        }
                    }";
            var runResult = ExecuteQuery(query, gqlInputs, root: null, userContext: GraphQLUserContext);
            bool bErrors = runResult.Errors?.Any() == true;
            Assert.IsFalse(bErrors);


            Dictionary<string, object> data = (Dictionary<string, object>)runResult.Data;
            var resultJson = JsonConvert.SerializeObject(data["blog"]);
            SimpleDocument<Blog> resultBlogDocument =
                JsonConvert.DeserializeObject<SimpleDocument<Blog>>(resultJson);
            blogEntry.TenantId = resultBlogDocument.TenantId;
            resultBlogDocument.TenantId = blogEntry.TenantId;
            resultBlogDocument.Document.EnableDeepCompare = blogEntry.Document.EnableDeepCompare;
            string additionalInfo = null;
            blogEntry.Document.EnableDeepCompare = true;
            resultBlogDocument.Document.EnableDeepCompare = true;
            resultBlogDocument.ShouldBe(blogEntry, additionalInfo);
        }

        public ExecutionResult CreateQueryResult(string result)
        {
            object expected = null;
            if (!string.IsNullOrWhiteSpace(result))
            {
                expected = JObject.Parse(result);
            }
            return new ExecutionResult { Data = expected };
        }
        public ExecutionResult AssertQuerySuccess(
            string query,
            string expected,
            Inputs inputs = null,
            object root = null,
            object userContext = null,
            CancellationToken cancellationToken = default(CancellationToken),
            IEnumerable<IValidationRule> rules = null)
        {
            var queryResult = CreateQueryResult(expected);
            return AssertQuery(query, queryResult, inputs, root, userContext, cancellationToken, rules);
        }
        public ExecutionResult AssertQueryWithErrors(
           string query,
           string expected,
           Inputs inputs = null,
           object root = null,
           object userContext = null,
           CancellationToken cancellationToken = default(CancellationToken),
           int expectedErrorCount = 0)
        {
            var queryResult = CreateQueryResult(expected);
            return AssertQueryIgnoreErrors(query, queryResult, inputs, root, userContext, cancellationToken, expectedErrorCount);
        }

        public ExecutionResult AssertQueryIgnoreErrors(
            string query,
            ExecutionResult expectedExecutionResult,
            Inputs inputs,
            object root,
            object userContext = null,
            CancellationToken cancellationToken = default(CancellationToken),
            int expectedErrorCount = 0)
        {
            var runResult = Executer.ExecuteAsync(Schema, root, query, null, inputs, userContext, cancellationToken).Result;

            var writtenResult = Writer.Write(new ExecutionResult { Data = runResult.Data });
            var expectedResult = Writer.Write(expectedExecutionResult);

            // #if DEBUG
            //             Console.WriteLine(writtenResult);
            // #endif

            writtenResult.ShouldBe(expectedResult);

            var errors = runResult.Errors ?? new ExecutionErrors();

            errors.Count().ShouldBe(expectedErrorCount);

            return runResult;
        }
        public ExecutionResult AssertQuery(
           string query,
           ExecutionResult expectedExecutionResult,
           Inputs inputs,
           object root,
           object userContext = null,
           CancellationToken cancellationToken = default(CancellationToken),
           IEnumerable<IValidationRule> rules = null)
        {
            var runResult = Executer.ExecuteAsync(
                Schema,
                root,
                query,
                null,
                inputs,
                userContext,
                cancellationToken,
                rules
            ).Result;

            var writtenResult = Writer.Write(runResult);
            var expectedResult = Writer.Write(expectedExecutionResult);

            // #if DEBUG
            //             Console.WriteLine(writtenResult);
            // #endif

            string additionalInfo = null;

            if (runResult.Errors?.Any() == true)
            {
                additionalInfo = string.Join(Environment.NewLine, runResult.Errors
                    .Where(x => x.InnerException is GraphQLSyntaxErrorException)
                    .Select(x => x.InnerException.Message));
            }

            writtenResult.ShouldBe(expectedResult, additionalInfo);

            return runResult;
        }



        public ExecutionResult ExecuteQuery(
         string query,
         Inputs inputs,
         object root,
         object userContext = null,
         CancellationToken cancellationToken = default(CancellationToken),
         IEnumerable<IPluginValidationRule> rules = null)
        {
            var fixedRules = rules != null ? rules.ToList() : new List<IPluginValidationRule>();
            var newRules = fixedRules.Concat(DocumentValidator.CoreRules());
            var runResult = Executer.ExecuteAsync(
                Schema,
                root,
                query,
                null,
                inputs,
                userContext,
                cancellationToken,
                newRules
            ).Result;

            var writtenResult = Writer.Write(runResult);

            // #if DEBUG
            //             Console.WriteLine(writtenResult);
            // #endif

            string additionalInfo = null;

            if (runResult.Errors?.Any() == true)
            {
                additionalInfo = string.Join(Environment.NewLine, runResult.Errors
                    .Where(x => x.InnerException is GraphQLSyntaxErrorException)
                    .Select(x => x.InnerException.Message));
            }

            return runResult;
        }

    }
}
