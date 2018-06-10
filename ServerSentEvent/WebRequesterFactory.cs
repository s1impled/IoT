namespace ServerSentEvent
{
    class WebRequesterFactory : IWebRequesterFactory
    {
        public IWebRequester Create()
        {
            return new WebRequester();
        }
    }
}
