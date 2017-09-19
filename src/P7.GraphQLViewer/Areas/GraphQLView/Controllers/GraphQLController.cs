using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Http;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using P7.Core.Localization;
using P7.GraphQLCore;
using P7.GraphQLCore.Validators;

namespace P7.GraphQLViewer.Areas.GraphQLView.Controllers
{
    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string NamedQuery { get; set; }
        public string Query { get; set; }
        public string Variables { get; set; }
    }

    [Route("api/[controller]")]
    [Produces("application/json")]
    public class GraphQLController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        private ILogger Logger { get; set; }
        private IDocumentExecuter _executer { get; set; }
        private IDocumentWriter _writer { get; set; }
        private ISchema _schema { get; set; }
        private readonly IDictionary<string, string> _namedQueries;
        private List<IPluginValidationRule> _pluginValidationRules;
        public GraphQLController(
            IHttpContextAccessor httpContextAccessor,
            ILogger<GraphQLController> logger,
            IDocumentExecuter executer,
            IDocumentWriter writer,
            ISchema schema,
            IEnumerable<IPluginValidationRule> pluginValidationRules )
        {
            _httpContextAccessor = httpContextAccessor;
            Logger = logger;
            _executer = executer;
            _writer = writer;
            _schema = schema;
            _namedQueries = new Dictionary<string, string>
            {
                ["a-query"] = @"query foo { hero { name } }"
            };
            _pluginValidationRules = pluginValidationRules.ToList();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]GraphQLQuery query)
        {
            var inputs = query.Variables.ToInputs();
            var queryToExecute = query.Query;

            if (!string.IsNullOrWhiteSpace(query.NamedQuery))
            {
                queryToExecute = _namedQueries[query.NamedQuery];
            }

            var result = await _executer.ExecuteAsync(_ =>
            {
                _.UserContext = new GraphQLUserContext(_httpContextAccessor);
                _.Schema = _schema;
                _.Query = queryToExecute;
                _.OperationName = query.OperationName;
                _.Inputs = inputs;
                _.ComplexityConfiguration = new ComplexityConfiguration { MaxDepth = 15 };
                _.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
                _.ValidationRules = _pluginValidationRules.Concat(DocumentValidator.CoreRules());

            }).ConfigureAwait(false);

            var httpResult = result.Errors?.Count > 0
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.OK;

            var json = _writer.Write(result);
            dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

            var rr =  new ObjectResult(obj) { StatusCode = (int)httpResult };
            return rr;
        }
    }

}
