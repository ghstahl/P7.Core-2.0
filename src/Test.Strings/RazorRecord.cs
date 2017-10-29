using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using P7.FileSystem.Utils;

namespace Test.Strings
{
    public class RazorRecord
    {
        public string Location { get; set; }

        public string Content { get; set; }
    }


    public class FileSystemEventSink : IFileSystemEventSink
    {
        public List<RazorRecord> RazorRecords = new List<RazorRecord>();
        private string _root;
        public void OnNewDirectory(string fullPath)
        {
            if (string.IsNullOrEmpty(_root))
            {
                _root = fullPath;
            }
        }

        public void OnNewFile(string fullPath)
        {
            var fi = new FileInfo(fullPath);
            if (fi.Extension == ".cshtml")
            {
                var sub = fullPath.Substring(_root.Length);
                var location = sub.Replace('\\', '/');
                var content = File.ReadAllText(fullPath);
                RazorRecords.Add(new RazorRecord(){Location = location, Content = content });
            }
        }
    }



    [TestClass]
    public class UnitTest1
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
