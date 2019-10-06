using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace QICore.ElasticSearchCore
{
    class Program
    {
        static void Main(string[] args)
        {          

            AutofacIoc.Register();
            test.write();
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
}
