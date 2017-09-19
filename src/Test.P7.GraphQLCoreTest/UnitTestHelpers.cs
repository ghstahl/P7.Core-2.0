using System.IO;
using System.Reflection;

namespace Test.P7.GraphQLCoreTest
{
    public class UnitTestHelpers
    {
        private static string _baseDir;
        public static string BaseDir
        {
            get
            {
                if (_baseDir == null)
                {
                    var assembly = typeof(UnitTestHelpers).GetTypeInfo().Assembly;
                    var codebase = assembly.CodeBase.Replace("file:///", "");
                    _baseDir = Path.GetDirectoryName(codebase);
                }
                return _baseDir;
            }
        }
    }
}