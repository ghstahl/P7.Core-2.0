using System;

namespace P7.Razor.FileProvider
{
    internal class EmptyDisposable : IDisposable
    {
        public static EmptyDisposable Instance { get; } = new EmptyDisposable();
        private EmptyDisposable() { }
        public void Dispose() { }
    }
}