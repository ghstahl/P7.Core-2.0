using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P7.Core.Writers;
using P7.Store;
using Shouldly;

namespace Test.P7.GraphQLCoreTest
{
    [TestClass]
    public class ByteStringTests
    {
        [TestMethod]
        public void paging_state_conversions()
        {
            PagingState pagingStateExpected = new PagingState() {CurrentIndex = 1234};
            var bytes = pagingStateExpected.Serialize();
            var pagingState = bytes.DeserializePageState();

            pagingState.ShouldBe(pagingStateExpected);

            var psString = Convert.ToBase64String(bytes);
            bytes = Convert.FromBase64String(psString);
            pagingState = bytes.DeserializePageState();

            pagingState.ShouldBe(pagingStateExpected);

            var urlEncodedPagingState = WebUtility.UrlEncode(psString);
            var psStringUrlDecoded = WebUtility.UrlDecode(urlEncodedPagingState);

            psStringUrlDecoded.ShouldBe(psString);
            bytes = Convert.FromBase64String(psStringUrlDecoded);
            pagingState = bytes.DeserializePageState();
            pagingState.ShouldBe(pagingStateExpected);
        }
    }
}
