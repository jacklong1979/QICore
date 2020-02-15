using Autofac;
using Autofac.Core;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QICore.ElasticSearchCore.WebApi.Dao;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QICore.ElasticSearchCore.WebApi.Common
{
   
    public class AutofacIoc
    {
        private static ContainerBuilder _builder = new ContainerBuilder();
        private static IContainer _container;

        /// <summary>
        /// 注册所有继承IDenpendency的接口.
        /// </summary>
        /// <returns></returns>
        public static ContainerBuilder Register()
        {
            //var configuration = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory())
            //.AddJsonFile("appsettings.json", true, true)
            //.AddJsonFile("appsettings.Development.json", true, true)
            //.Build();
            //IServiceCollection services = new ServiceCollection();

            //ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");
            //XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));  
            var builder = new ContainerBuilder();
            Type basetype = typeof(IBase);
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => basetype.IsAssignableFrom(t) && t.IsClass)
                .AsImplementedInterfaces().InstancePerLifetimeScope();
            _builder = builder;
            _container = builder.Build();
            return builder;
        }

        /// <summary>
        /// 注册一个单例实体.
        /// </summary>
        /// <typeparam name="T">类对象.</typeparam>
        /// <param name="instance">实例对象.</param>
        public static void Register<T>(T instance)
            where T : class
        {
            _builder.RegisterInstance(instance).SingleInstance();
        }

        /// <summary>
        /// 注册.
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <typeparam name="TInterface"></typeparam>
        public static void Register<TClass,TInterface>() 
        {
            _builder.RegisterType<TClass>().As<TInterface>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public static T Resolve<T>(params Parameter[] parameters)
        {
            return _container.Resolve<T>(parameters);
        }

        public static object Resolve(Type targetType)
        {
            return _container.Resolve(targetType);
        }

        public static object Resolve(Type targetType, params Parameter[] parameters)
        {
            return _container.Resolve(targetType, parameters);
        }
    }
}
