using System;

namespace RET.Interfaces
{
    public interface IGpioAppliance : IDisposable
    {
        int GpioPin { get; }

        void Initialize();

        void TurnApplianceOn();

        void TurnApplianceOff();
    }
}
