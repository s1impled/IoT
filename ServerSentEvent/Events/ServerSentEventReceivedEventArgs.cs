using System;

namespace ServerSentEvent.Events
{
    public class ServerSentEventReceivedEventArgs : EventArgs
    {
        public ServerSentEvent Message { get; private set; }

        public ServerSentEventReceivedEventArgs(ServerSentEvent message)
        {
            this.Message = message;
        }
    }
}
