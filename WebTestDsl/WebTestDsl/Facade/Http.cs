using System;
using System.Net;
using System.Net.Http;

namespace WebTestDsl.Facade
{
    public static class Http
    {
        private static HttpRequestMessage _request = new HttpRequestMessage();
        private static readonly CookieCollection _cookieCollection = new CookieCollection();
        public static void SetCookie(string name, string value)
        {
            _cookieCollection.Add(new Cookie(name, value));
        }

        public static void SetHeader(string name, string value)
        {
            _request.Headers.Add(name, value);
        }

        public static Response Get(string url)
        {
            _request.Method = HttpMethod.Get;
            _request.RequestUri = new Uri(url);
            return Execute();
        }

        private static Response Execute()
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
