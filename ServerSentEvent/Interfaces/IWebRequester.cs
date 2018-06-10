using System;
using System.Threading.Tasks;

namespace ServerSentEvent
{
    public interface IWebRequester
    {
        Task<IServerResponse> Get(Uri url, string authToken);

    }
}
