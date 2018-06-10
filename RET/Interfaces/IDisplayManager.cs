using System;
using Windows.Foundation;

namespace RET.Interfaces
{
    public interface IDisplayManager : IDisposable
    {
        TimeSpan DisplayTimeout { get; set; }

        bool IsDisplayOn { get; }

        bool DisplayHeatIcon { get; set; }

        IAsyncOperation<bool> Initialize();

        void TurnOff();

        void TurnOn();

        void WriteLine(string format, params object[] args);
    }
}
