using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using P7.Core.Settings;
using P7.GraphQLCore.Stores;

namespace P7.GraphQLCore.Validators
{
    interface ICurrentEnterLeaveListenerState
    {
        EnterLeaveListenerState EnterLeaveListenerState { get; }
    }

    public interface IPluginValidationRule: IValidationRule { }
    public class TestValidationRule : IPluginValidationRule
    {
        class MyEnterLeaveListenerSink : IEnterLeaveListenerEventSink, ICurrentEnterLeaveListenerState
        {
            public EnterLeaveListenerState EnterLeaveListenerState { get; private set; }

            public void OnEvent(EnterLeaveListenerState enterLeaveListenerState)
            {
                EnterLeaveListenerState = enterLeaveListenerState;
            }
        }

        private List<IGraphQLAuthorizationCheck> _graphQLAuthorizationChecks;
        private List<IGraphQLClaimsAuthorizationCheck> _graphQLClaimsAuthorizationChecks;
        private IGraphQLFieldAuthority _graphQLFieldAuthority;
       
        public TestValidationRule( IGraphQLFieldAuthority graphQLFieldAuthority)
        {
          
            _graphQLFieldAuthority = graphQLFieldAuthority;
        }
        
        public INodeVisitor Validate(ValidationContext context)
        {
            var userContext = context.UserContext.As<GraphQLUserContext>();
            var user = userContext.HttpContextAccessor.HttpContext.User;

            IEnumerable<string> claimsEnumerable = (from item in user.Claims
                let c = item.Type
                select c).ToList();
//            IEnumerable<string> claimsEnumerable = query.ToList();
            var authenticated = user?.Identity.IsAuthenticated ?? false;
            var myEnterLeaveListenerSink = new MyEnterLeaveListenerSink();
            var currentEnterLeaveListenerState = (ICurrentEnterLeaveListenerState) myEnterLeaveListenerSink;
            var myEnterLeaveListener = new MyEnterLeaveListener(_ =>
            {
                
                _.Match<Operation>(op =>
                {
                    if (!authenticated)
                    {
                        var usages = context.GetRecursiveVariables(op).Select(usage => usage.Node.Name);
                     
                        var selectionSet = op.SelectionSet;
                        foreach (var selection in selectionSet.Selections)
                        {
                            var d = selection;
                            var dd = selection.ToString();
                            
                        }
                        /*
                        var queryQ = from item in selectionSet.Selections
                            where _settings.Value.Query.OptOut.Contains(item.)
                            select item;
                            */

                    }
                    var opType = op.OperationType;
                    var query = from item in op.SelectionSet.Selections
                        select ((GraphQL.Language.AST.Field) item).Name;
                    if (op.OperationType == OperationType.Mutation && !authenticated)
                    {
                        context.ReportError(new ValidationError(
                            context.OriginalQuery,
                            "auth-required",
                            $"Authorization is required to access {op.Name}.",
                            op));
                    }



                });

                _.Match<Field>(fieldAst =>
                {
                    var currentFieldPath = currentEnterLeaveListenerState.EnterLeaveListenerState.CurrentFieldPath;
                    var currentOperationType = currentEnterLeaveListenerState.EnterLeaveListenerState.OperationType;
                    var requiredClaims = _graphQLFieldAuthority
                        .FetchRequiredClaimsAsync(currentOperationType, currentFieldPath).Result;
                    var rcQuery = (from requiredClaim in requiredClaims
                        let c = requiredClaim.Type
                        select c).ToList();
                    var canAccess = requiredClaims.All(x =>
                    {
                        var result = false;
                        foreach (var ce in user.Claims)
                        {
                            if (ce.Type == x.Type)
                            {
                                if (string.IsNullOrEmpty(x.Value))
                                {
                                    result = true;
                                }
                                else
                                {
                                    result = x.Value == ce.Value;
                                }
                            }
                        }
                        return result;
                    });

                  //  var canAccess = rcQuery.All(x => claimsEnumerable?.Contains(x) ?? false);
                    if (!canAccess)
                    {
                        context.ReportError(new ValidationError(
                            context.OriginalQuery,
                            "auth-required",
                            $"You are not authorized to run this query.",
                            fieldAst));
                    }
                });
                /*
                // this could leak info about hidden fields in error messages
                // it would be better to implement a filter on the schema so it
                // acts as if they just don't exist vs. an auth denied error
                // - filtering the schema is not currently supported
                _.Match<Field>(fieldAst =>
                {
                    var fieldDef = context.TypeInfo.GetFieldDef();
                    if (fieldDef.RequiresPermissions() &&
                        (!authenticated || !fieldDef.CanAccess(userContext.User.Claims)))
                    {
                        context.ReportError(new ValidationError(
                            context.OriginalQuery,
                            "auth-required",
                            $"You are not authorized to run this query.",
                            fieldAst));
                    }
                });
                */
            });
            myEnterLeaveListener.RegisterEventSink(myEnterLeaveListenerSink);
            return myEnterLeaveListener;
        }
    }

    public class RequiresAuthValidationRule : IValidationRule
    {
        public INodeVisitor Validate(ValidationContext context)
        {
            var userContext = context.UserContext.As<GraphQLUserContext>();
            var user = userContext.HttpContextAccessor.HttpContext.User;


            var authenticated = user?.Identity.IsAuthenticated ?? false;

            return new EnterLeaveListener(_ =>
            {
                _.Match<Operation>(op =>
                {
                    if (op.OperationType == OperationType.Mutation && !authenticated)
                    {
                        context.ReportError(new ValidationError(
                            context.OriginalQuery,
                            "auth-required",
                            $"Authorization is required to access {op.Name}.",
                            op));
                    }
                });
                /*
                // this could leak info about hidden fields in error messages
                // it would be better to implement a filter on the schema so it
                // acts as if they just don't exist vs. an auth denied error
                // - filtering the schema is not currently supported
                _.Match<Field>(fieldAst =>
                {
                    var fieldDef = context.TypeInfo.GetFieldDef();
                    if (fieldDef.RequiresPermissions() &&
                        (!authenticated || !fieldDef.CanAccess(userContext.User.Claims)))
                    {
                        context.ReportError(new ValidationError(
                            context.OriginalQuery,
                            "auth-required",
                            $"You are not authorized to run this query.",
                            fieldAst));
                    }
                });
                */
            });
        }
    }
}
