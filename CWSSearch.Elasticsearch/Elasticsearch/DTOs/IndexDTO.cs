using System;
using System.Collections.Generic;
using System.Text;

namespace CWSSearch.Elasticsearch.DTOs
{
    public class IndexDTO
    {
        public int Id { get; set; }

        public string LocationName { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
    }
}
