using CWSSearch.Elasticsearch.Concretes;
using CWSSearch.Elasticsearch.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWSSearch.Test
{
    [TestClass]
    public class ElasticsearchTests
    {
        private const string INDEX_NAME = "testxcwscontent";

        [TestMethod]
        public async Task Content_Should_Be_Indexed()
        {
            var es = new ElasticsearchService(INDEX_NAME);

            var res = new List<bool>
            {
                await es.IndexAsync(new IndexDTO()
                {
                    Id = 1,
                    LocationName = "Ankara",
                    Title = "Acil ekipman aranıyor",
                    Detail = "",
                })
            };

            Assert.IsTrue(res.Count(t => t == true) == res.Count);
        }

        [TestMethod]
        [DataRow("acıl")]
        [DataRow("acilll")]
        public async Task Should_Find_For_Words(string keyword)
        {
            var es = new ElasticsearchService(INDEX_NAME);

            var res1 = await es.SearchAsync(keyword);

            Assert.IsTrue(res1.Total > 0);
        }
    }
}
