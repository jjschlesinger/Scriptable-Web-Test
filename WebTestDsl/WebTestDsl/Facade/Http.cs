using System;
using System.Net;
using System.Net.Http;

namespace WebTestDsl.Facade
{
    public class Http
    {
        private HttpRequestMessage _request = new HttpRequestMessage();
        private readonly CookieCollection _cookieCollection = new CookieCollection();

        public static Http Create()
        {
            return new Http();
        }
        
        public Http SetCookie(string name, string value)
        {
            _cookieCollection.Add(new Cookie(name, value));
            return this;
        }

        public Http SetHeader(string name, string value)
        {
            _request.Headers.Add(name, value);
            return this;
        }

        public Response Get(string url)
        {
            _request.Method = HttpMethod.Get;
            _request.RequestUri = new Uri(url);
            return Execute();
        }

        private Response Execute()
        {
            foreach (Cookie cookie in _cookieCollection)
            {
                cookie.Domain = _request.RequestUri.Host;
            }
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(_cookieCollection);
            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer})
            using (var httpClient = new HttpClient(handler))
            {
                var hrm = httpClient.SendAsync(_request).Result;
                var resp = new Response { Status = Convert.ToInt32(hrm.StatusCode), Body = hrm.Content.ReadAsStringAsync().Result };
                _request = new HttpRequestMessage();
                return resp;
            }
        }
    }
}
