using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.ElasticSearchCore.WebApi.Common
{
    public class ElasticSearchHelper
    {
       
        /// <summary>
        /// 如果同名索引不存在则创建索引
        /// </summary>
        /// <param name="client">ElasticClient实例</param>
        /// <param name="indexName">要创建的索引名称</param>
        /// <param name="numberOfReplicas">默认副本数量，如果是单实例，注意改成0</param>
        /// <param name="numberOfShards">默认分片数量</param>
        /// <returns></returns>
        public static bool CreateIndex<T>(IElasticClient client, string indexName = "wizplant", int numberOfReplicas = 1, int numberOfShards = 5) where T : class
        {
            var existsResponse = client.Indices.Exists(indexName);
            // 存在则返回true 不存在创建
            if (existsResponse.Exists)
            {
                return true;
            }
            var indexState = new IndexState
            {
                Settings = new IndexSettings
                {
                    NumberOfReplicas = numberOfReplicas, //副本数
                    NumberOfShards = numberOfShards //分片数
                }
            };

            if (string.IsNullOrWhiteSpace(indexName))
            {
                indexName = typeof(T).Name.ToLower();
            }
            CreateIndexResponse response = client.Indices.Create(indexName, p => p.InitializeUsing(indexState).Map<T>(r => r.AutoMap()));

         // var result = client.CreateIndex(indexName, c => c.InitializeUsing(indexState).Mappings(ms => ms.Map<T>(m => m.AutoMap())));
            return response.IsValid;
        }
        /// <summary>
        /// 获取索引ElasticClient对象. 
        /// </summary>
        /// <param name="url">如：http://localhost:9200</param>
        /// <param name="indexName">索引名称（要小写）</param>
        /// <returns></returns>
        public static ElasticClient GetElasticClient(string url,string indexName = "wizplant")
        {            
            var node = new Uri(url);
            ConnectionSettings connectionSettings = new ConnectionSettings(node);
            ConnectionSettings settings = connectionSettings.DefaultIndex(indexName);
            var client = new ElasticClient(settings);
          
            return client;
        }
        /// <summary>
        /// 新增文档
        /// </summary>
        /// <param name="client">ElasticClient</param>
        /// <param name="obj">文档对象</param>
        public static IndexResponse Add(ElasticClient client,object obj)
        {
            var indexResponse = client.IndexDocument(obj);
            return indexResponse;
        }
        /// <summary>
        /// 新增文档
        /// </summary>
        /// <param name="client">ElasticClient</param>
        /// <param name="obj">文档对象</param>
        public static async Task<IndexResponse> AddAsync(ElasticClient client, object obj)
        {
            var indexResponse =await client.IndexDocumentAsync(obj);
            return indexResponse;
        }
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <returns></returns>
        public static List<T> GetAll<T>(ElasticClient client) where T:class
        {
            var searchResults = client.Search<T>(s => s
                .From(0)
                .Size(int.MaxValue)
                );
            return searchResults.Documents.ToList();
        }
    }
}
