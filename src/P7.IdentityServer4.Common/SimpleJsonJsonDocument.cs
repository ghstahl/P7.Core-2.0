using Newtonsoft.Json;

namespace P7.IdentityServer4.Common
{
    public class SimpleJsonJsonDocument<T> : ISimpleJsonDocument where T : class
    {
        private readonly T _document;

        public SimpleJsonJsonDocument(T document)
        {
            _document = document;
        }

        public SimpleJsonJsonDocument(string documentJson)
        {
            _document = JsonConvert.DeserializeObject<T>(documentJson);
        }

        public object Document => _document;

        public string DocumentJson
        {
            get
            {
                string output = JsonConvert.SerializeObject(_document);
                return output;
            }
        }
    }
}