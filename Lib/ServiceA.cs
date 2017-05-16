using System;
using System.Diagnostics;

namespace Lib
{
    public interface IServiceA
    {
        void Fu();
    }

    public class ServiceA : IServiceA, IDisposable
    {
        public void Fu()
        {
            Debug.WriteLine("Fu** you!");
        }

        public void Dispose()
        {
            Debug.WriteLine("DISPOSE!");
        }
    }
}
