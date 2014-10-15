using System;
using HttpFacade;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebTestDsl.Tests
{
    [TestClass]
    public class HttpTests
    {
        [TestMethod]
        public void Get()
        {
            var resp = Http.Create().Get("http://www.google.com");
            Assert.AreEqual(resp.Status, 200);
        }
    }
}
