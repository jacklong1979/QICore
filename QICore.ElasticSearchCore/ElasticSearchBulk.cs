
using Nest;
using System;
using System.Collections.Generic;
using System.Threading;
using static System.Console;

namespace QICore.ElasticSearchCore
{
    public class ElasticSearchBulk
    {
        private const string Format = "BulkHotelGeo Error : {0}";

        public static bool CreateIndex<T>(IElasticClient elasticClient, string indexName) where T : class
        {
            var existsResponse = elasticClient.Indices.Exists(indexName);
            // 存在则返回true 不存在创建
            if (existsResponse.Exists)
            {
                return true;
            }
            //基本配置
            IIndexState indexState = new IndexState
            {
                Settings = new IndexSettings
                {
                    NumberOfReplicas = 1, // 副本数
                    NumberOfShards = 6, // 分片数
                },
            };

            CreateIndexResponse response = elasticClient.Indices.Create(indexName, p => p
                .InitializeUsing(indexState).Map<T>(r => r.AutoMap())
            );

            return response.IsValid;
        }

        /// <summary>
        /// ElasticClient.
        /// </summary>
        /// <returns>返回ElasticClient</returns>
        public static ElasticClient GetElasticClient()
        {
            var client = new ElasticClient();
            return client;
        }

        /// <summary>
        /// 批量插入.
        /// </summary>
        /// <typeparam name="T">对象.</typeparam>
        /// <param name="elasticClient">IElasticClient.</param>
        /// <param name="indexName">索引名称.</param>
        /// <param name="list">对象列表.</param>
        /// <returns>返回成功或失败.</returns>
        public static bool BulkAll<T>(IElasticClient elasticClient, IndexName indexName, IEnumerable<T> list)
            where T : class
        {
            const int size = 1000;
            var tokenSource = new CancellationTokenSource();
            var observableBulk = elasticClient.BulkAll(list, f => f
                    .MaxDegreeOfParallelism(8)
                    .BackOffTime(TimeSpan.FromSeconds(10))
                    .BackOffRetries(2)
                    .Size(size)
                    .RefreshOnCompleted()
                    .Index(indexName)
                    .BufferToBulk((r, buffer) => r.IndexMany(buffer))
                , tokenSource.Token);

            var countdownEvent = new CountdownEvent(1);

            Exception exception = null;

            void OnCompleted()
            {
                WriteLine("BulkAll Finished");
                countdownEvent.Signal();
            }

            var bulkAllObserver = new BulkAllObserver(
                onNext: response =>
                {
                    WriteLine($"Indexed {response.Page * size} with {response.Retries} retries");
                },
                onError: ex =>
                {
                    WriteLine("BulkAll Error : {0}", ex);
                    exception = ex;
                    countdownEvent.Signal();
                },
                onCompleted: OnCompleted);

            observableBulk.Subscribe(bulkAllObserver);

            countdownEvent.Wait(tokenSource.Token);

            if (exception != null)
            {
                WriteLine(Format, arg0: exception);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
