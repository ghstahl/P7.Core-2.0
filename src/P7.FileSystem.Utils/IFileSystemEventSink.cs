using System;
using System.Diagnostics.Tracing;

namespace P7.FileSystem.Utils
{
    public interface IFileSystemEventSink
    {
        void OnNewDirectory(string fullPath);
        void OnNewFile(string fullPath);
    }
}
