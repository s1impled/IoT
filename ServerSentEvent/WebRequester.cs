using System;
using System.Net;
using System.Threading.Tasks;

namespace ServerSentEvent
{
    class WebRequester : IWebRequester
    {
        public Task<IServerResponse> Get(Uri url, string authToken)
        {
            HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create(url);
            wreq.Method = "GET";
            wreq.Proxy = null;
            if (!string.IsNullOrEmpty(authToken))
            {
                wreq.Headers.Add(HttpRequestHeader.Authorization, string.Format("Bearer {0}", authToken));
            }
            wreq.Headers.Add(HttpRequestHeader.Accept, "text/event-stream");

            var taskResp = Task.Factory.FromAsync<WebResponse>(wreq.BeginGetResponse,
                                                            wreq.EndGetResponse,
                                                            null).ContinueWith<IServerResponse>(t => new ServerResponse(t.Result));
            return taskResp;
        }
    }
}
