using ServerSentEvent.Interfaces;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSentEvent
{
    class ConnectingState : IConnectionState
    {
        private Uri mUrl;
        private string mAuthToken;
        private IWebRequesterFactory mWebRequesterFactory;
        public EventSourceState State { get { return EventSourceState.CONNECTING; } }
        
        public ConnectingState(Uri url, string authToken, IWebRequesterFactory webRequesterFactory)
        {
            mUrl = url ?? throw new ArgumentNullException("Url cant be null");
            mWebRequesterFactory = webRequesterFactory ?? throw new ArgumentNullException("Factory cant be null");
            mAuthToken = authToken;
        }

        public Task<IConnectionState> Run(Action<ServerSentEvent> donothing, CancellationToken cancelToken)
        {
            IWebRequester requester = mWebRequesterFactory.Create();
            var taskResp = requester.Get(mUrl, mAuthToken);

            return taskResp.ContinueWith<IConnectionState>(tsk => 
            {
                if (tsk.Status == TaskStatus.RanToCompletion && !cancelToken.IsCancellationRequested)
                {
                    IServerResponse response = tsk.Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return new ConnectedState(response, mWebRequesterFactory);
                    }
                    else
                    {
                        Debug.WriteLine("Failed to connect to: " + mUrl.ToString() + response ?? (" Http statuscode: " + response.StatusCode));
                    }
                }

                return new DisconnectedState(mUrl, mAuthToken, mWebRequesterFactory);
            });
        }
    }
}
