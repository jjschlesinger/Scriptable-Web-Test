using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebTestDsl.Scripting;

namespace WebTestDsl.Tests
{
    [TestClass]
    public class DSLParserTests
    {
        [TestMethod]
        public void ToCSharp()
        {
            const string expectedCsharp =
                "var http = Http.Create();\r\nhttp.SetCookie(\"myCookieName\", \"myCookieValue\");\r\nhttp.SetHeader(\"accept\", \"text/html\");\r\nvar resp =http.Get(\"http://google.com\");\r\nif(resp.Status == 200) \r\n{ \r\n\thttp.SetHeader(\"content-type\", \"application/x-www-form-urlencoded\");\r\n\tresp = Http.Post(\"http://google.com\", \"firstname=josh&lastname=schlesinger\");\r\n} ";
            		
            const string dsl = "http.set_cookie \"myCookieName\" \"myCookieValue\"\r\n" +
                               "http.set_header \"accept\" \"text/html\"\r\n" +
                               "var resp = http.get \"http://google.com\"\r\n" +
                               "when resp.status is 200 then\r\n" +
                               "begin\r\n" +
                               "\thttp.set_header \"content-type\" \"application/x-www-form-urlencoded\"\r\n" + 
                               "\tresp = http.post \"http://google.com\" \"firstname=josh&lastname=schlesinger\"\r\n" + 
                               "end";

            var csharpLines = DSLParser.ToCSharp(dsl.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
            var actualCsharp = csharpLines.Aggregate((x, y) => x + "\r\n" + y);

            Assert.AreEqual(expectedCsharp, actualCsharp);
        }
    }
}
