using System;
using System.IO;
using System.Net;

namespace ServerSentEvent
{
    public interface IServerResponse
    {
        HttpStatusCode StatusCode { get; }

        Stream GetResponseStream();

        Uri ResponseUri { get; }
    }
}
