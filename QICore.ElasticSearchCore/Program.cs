using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using System.Collections.Generic;
using System.IO;

namespace QICore.ElasticSearchCore
{
    class Program
    {
        static void Main(string[] args)
        {          

            AutofacIoc.Register();
            test.write();

            const int numberOfCycles = 10;
            var node = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(
                node
            ).DefaultIndex("people");
            var client = new ElasticClient(settings);
            //var searchResults = client.Search<Person>(s => s
            //    .From(0)
            //    .Size(10)
            //    .Query(q => q
            //         .Term(p => p.Firstname, "martijn")
            //    )
            //);
           // var client = ElasticSearchBulk.GetElasticClient();
            //var indexName = "people";//索引名称小写
            //var createSuccess = ElasticSearchBulk.CreateIndex<TestNum>(client, indexName);
            //if (createSuccess)
            //{
               

             
            //}
            var list = new List<TestNum>();
            for (var i = 5; i < numberOfCycles; i++)
            {
                var item = new TestNum { id = i, name = $"中国人民: {i} ", age = 10 * i };
                list.Add(item);
            }
           // client.IndexMany<TestNum>(list);
           ElasticSearchBulk.BulkAll(client, "people", list);

            Console.Read();
        }
    }
    public interface IFunction1 : IDenpendency
    {
        string GetName();
    }
    public class Funciton1 : IFunction1
    {
        public string GetName()
        {
            return "功能点1";
        }
    }
    public class TestNum
    {
        public int id { get; set; }
        public string name { get; set; }
        public int age { get; set; }
    }
}
