using System;

namespace ServerSentEvent.Events
{
    public class StateChangedEventArgs : EventArgs
    {
        public EventSourceState State { get; private set; }

        public StateChangedEventArgs(EventSourceState state)
        {
            this.State = state;
        }
    }
}
