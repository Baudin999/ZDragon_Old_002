using System;
using System.Collections.Generic;
using System.Text;

namespace CLI.Signals
{
    public sealed class SignalSingleton
    {
        public static Signal ExitSignal { get; set; } = new Signal();
    }
}
