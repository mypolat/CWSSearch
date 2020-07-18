using CWSSearch.Elasticsearch.Abstracts;
using CWSSearch.Elasticsearch.DTOs;
using Nest;
using System;
using System.Threading.Tasks;

namespace CWSSearch.Elasticsearch.Concretes
{
    public class ElasticsearchService : IElasticsearchService
    {
        private const string DEFAULT_ELASTICSEARCH_PROTOCOL = "http";
        private const string DEFAULT_ELASTICSEARCH_URL = "localhost";
        private const string DEFAULT_ELASTICSEARCH_PORT = "9200";
        private const string DEFAULT_CONTENT_INDEX_NAME = "cwscontent";

        private string DefaultIndexName { get; set; }

        private ElasticClient _elasticClient;

        public ElasticsearchService(string defaultIndexName = DEFAULT_CONTENT_INDEX_NAME)
        {
            DefaultIndexName = defaultIndexName;

            var node = new Uri($"{DEFAULT_ELASTICSEARCH_PROTOCOL}://{DEFAULT_ELASTICSEARCH_URL}:{DEFAULT_ELASTICSEARCH_PORT}");
            var settings = new ConnectionSettings(node);

            _elasticClient = new ElasticClient(settings);

            // Eğer index oluşturulmadıysa Index oluştur
            var existsResponse = _elasticClient.Indices.Exists(DefaultIndexName);

            if (!existsResponse.Exists)
            {
                var response = _elasticClient.Indices.Create(DefaultIndexName,
                        index => index.Map<IndexDTO>(
                            x => x
                            .Properties(ps => ps.Text(s => s.Name(n => n.Title).Analyzer("simple")))
                            .Properties(ps => ps.Text(s => s.Name(n => n.Detail).Analyzer("simple")))
                            .Properties(ps => ps.Text(s => s.Name(n => n.LocationName).Analyzer("simple")))
                        ));
            }
        }

        public async Task<bool> IndexAsync(IndexDTO indexDTO = null)
        {
            try
            {
                if (indexDTO != null)
                {
                    // Gelen içerik ile index oluşturulur
                    var resIndex = await _elasticClient.IndexAsync(indexDTO, idx => idx.Index(DefaultIndexName));

                    // Index oluşma sonucu döner
                    var res = resIndex.IsValid;

                    return res;
                }
                else
                {
                    throw new ArgumentNullException("IndexDTO cannot be null.");
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ISearchResponse<IndexDTO>> SearchAsync(string keyword = "")
        {
            try
            {
                if (string.IsNullOrEmpty(keyword))
                {
                    throw new ArgumentNullException("Keyword cannot be null or empty.");
                }

                // Verilen index içinde FuzzySearch arama yapar
                var result = await _elasticClient.SearchAsync<IndexDTO>(s => s
                    .Index(DefaultIndexName)
                    .Query(q => q
                        .MultiMatch(m => m
                        .Fields(f => f.Field(k => k.Title).Field(k => k.Detail).Field(k => k.LocationName))
                        .Query(keyword)
                        .Fuzziness(Fuzziness.Auto)
                        .FuzzyTranspositions(true)
                        .Analyzer("standard")
                        .PrefixLength(0)
                        )));

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
