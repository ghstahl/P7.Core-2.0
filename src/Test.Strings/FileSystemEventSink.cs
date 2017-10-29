using System.Collections.Generic;
using System.IO;
using P7.FileSystem.Utils;

namespace Test.Strings
{
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
}