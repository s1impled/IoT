using ServerSentEvent.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSentEvent
{ 
    class DisconnectedState : IConnectionState 
    {
        private Uri mUrl;
        private string mAuthToken;
        private IWebRequesterFactory mWebRequesterFactory;
        public EventSourceState State
        {
            get { return EventSourceState.CLOSED; }
        }

        public DisconnectedState(Uri url, string authToken, IWebRequesterFactory webRequesterFactory)
        {
            mUrl = url ?? throw new ArgumentNullException("Url cant be null");
            mWebRequesterFactory = webRequesterFactory;
            mAuthToken = authToken;
        }

        public Task<IConnectionState> Run(Action<ServerSentEvent> donothing, CancellationToken cancelToken)
        {
            if(cancelToken.IsCancellationRequested)
                return Task.Factory.StartNew<IConnectionState>(() => { return new DisconnectedState(mUrl, mAuthToken, mWebRequesterFactory); });
            else
                return Task.Factory.StartNew<IConnectionState>(() => { return new ConnectingState(mUrl, mAuthToken, mWebRequesterFactory); });
        }
    }
}
