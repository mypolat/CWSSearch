using CWSSearch.Elasticsearch.DTOs;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CWSSearch.Elasticsearch.Abstracts
{
    public interface IElasticsearchService
    {
        public Task<bool> IndexAsync(IndexDTO indexDTO);
        public Task<ISearchResponse<IndexDTO>> SearchAsync(string keyword);
    }
}
