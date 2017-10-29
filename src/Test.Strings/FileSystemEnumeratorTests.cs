using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P7.FileSystem.Utils;

namespace Test.Strings
{
    [TestClass]
    public class FileSystemEnumeratorTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var currentDirectory = Path.Combine(Directory.GetCurrentDirectory(), "assets");
            var fileSystemEnumerator = new FileSystemEnumerator(currentDirectory);

            var mySink = new FileSystemEventSink();
            fileSystemEnumerator.RegisterEventSink(mySink);

            fileSystemEnumerator.Start();

        }
    }
}