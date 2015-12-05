using System;
using System.Net;

namespace SubsDownloader
{
    public class MyWebClient : WebClient
    {
        private CookieContainer cookie;

        public CookieContainer Cookie { get { return cookie; } }

        public MyWebClient()
        {
            cookie = new CookieContainer();
        }

        public MyWebClient(CookieContainer givenContainer)
        {
            cookie = givenContainer;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = cookie;
            }
            return request;
        }
    }
}
