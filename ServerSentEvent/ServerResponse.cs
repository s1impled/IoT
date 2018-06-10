using System;
using System.Net;

namespace ServerSentEvent
{
    class ServerResponse : IServerResponse
    {
        private System.Net.HttpWebResponse mHttpResponse;

        public ServerResponse(System.Net.WebResponse webResponse)
        {
            this.mHttpResponse = webResponse as HttpWebResponse;
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return mHttpResponse.StatusCode;
            }
        }

        public System.IO.Stream GetResponseStream()
        {
            return mHttpResponse.GetResponseStream();
        }

        public Uri ResponseUri
        {
            get
            {
                return mHttpResponse.ResponseUri;
            }
        }
    }
}
