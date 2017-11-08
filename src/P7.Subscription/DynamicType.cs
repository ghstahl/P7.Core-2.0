using GraphQL.Language.AST;
using GraphQL.Types;

namespace P7.Subscription
{
    public class DynamicType : ScalarGraphType 
    {
        public DynamicType()
        {
            Name = "value";
        }

        public override object Serialize(object value)
        {
            return value;
        }

        public override object ParseValue(object value)
        {
            return value;
        }

        public override object ParseLiteral(IValue value)
        {
            var objectValue = value as ObjectValue;
            return objectValue;
        }
    }
}