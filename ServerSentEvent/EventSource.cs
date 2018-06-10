using ServerSentEvent.Events;
using ServerSentEvent.Interfaces;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ServerSentEvent
{
    public class EventSource
    {
        private int timeout;
        private IConnectionState mCurrentState = null;
        private CancellationToken mStopToken;
        private CancellationTokenSource mTokenSource = new CancellationTokenSource();
        private IWebRequesterFactory _webRequesterFactory = new WebRequesterFactory();

        public event EventHandler<StateChangedEventArgs> StateChanged;
        public event EventHandler<ServerSentEventReceivedEventArgs> EventReceived;
        
        public EventSourceState State { get { return CurrentState.State; } }
        private IConnectionState CurrentState
        {
            get { return mCurrentState; }
            set
            {
                if (!value.Equals(mCurrentState))
                {
                    StringBuilder sb = new StringBuilder("State changed from ");
                    sb.Append(mCurrentState == null ? "Unknown" : mCurrentState.State.ToString());
                    sb.Append(" to ");
                    sb.Append(value == null ? "Unknown" : value.State.ToString());
                    Debug.WriteLine(sb.ToString());
                    mCurrentState = value;
                    OnStateChanged(mCurrentState.State);
                }
            }
        }

        public Uri Url { get; private set; }

        public string AuthToken { get; private set; }

        public EventSource (Uri remoteAddress, string authToken = null, int timeout = (6000))
        {
            this.timeout = timeout;
            this.Url = remoteAddress;
            this.AuthToken = authToken;
            CurrentState = new DisconnectedState(Url, AuthToken, _webRequesterFactory);
            Debug.WriteLine("EventSource created for " + Url.ToString());
        }

        public void Start(CancellationToken stopRequest)
        {
            if (this.State == EventSourceState.CLOSED)
            {
                mStopToken = stopRequest;
                mTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stopRequest);
                this.Run();
            }
        }

        protected void Run()
        {
            if (mTokenSource.IsCancellationRequested && CurrentState.State == EventSourceState.CLOSED)
                return;

            mCurrentState.Run(this.OnEventReceived, mTokenSource.Token).ContinueWith(cs =>
            {
                CurrentState = cs.Result;
                Run();
            });
        }

        protected void OnEventReceived(ServerSentEvent sse)
        {
            if (EventReceived != null)
            {
                EventReceived(this, new ServerSentEventReceivedEventArgs(sse));
            }
        }

        protected void OnStateChanged(EventSourceState newState)
        {
            if (StateChanged != null)
            {
                StateChanged(this, new StateChangedEventArgs(newState));
            }
        }
    }
}
