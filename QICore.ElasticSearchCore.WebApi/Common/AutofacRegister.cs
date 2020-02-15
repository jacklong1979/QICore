using Autofac;
using QICore.ElasticSearchCore.WebApi.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace QICore.ElasticSearchCore.WebApi.Common
{
    public class AutofacModuleRegister : Autofac.Module
    {
        //重写Autofac管道Load方法，在这里注册注入
        protected override void Load(ContainerBuilder builder)
        {
            //必须是Service结束的
            //builder.RegisterAssemblyTypes(GetAssemblyByName("BlogService")).Where(a => a.Name.EndsWith("Service")).AsImplementedInterfaces();
            //builder.RegisterAssemblyTypes(GetAssemblyByName("BlogRepository")).Where(a => a.Name.EndsWith("Repository")).AsImplementedInterfaces();
            //单一注册
            //   builder.RegisterType<PersonService>().Named<IPersonService>(typeof(PersonService).Name);

          
            Type basetype = typeof(IBase);
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => basetype.IsAssignableFrom(t) && t.IsClass)
                .AsImplementedInterfaces().InstancePerLifetimeScope();
           

            //builder.RegisterType<Test>().As<ITest>();
        }
        /// <summary>
        /// 根据程序集名称获取程序集
        /// </summary>
        /// <param name="AssemblyName">程序集名称</param>
        public static Assembly GetAssemblyByName(String AssemblyName)
        {
            return Assembly.Load(AssemblyName);
        }
    }
}
