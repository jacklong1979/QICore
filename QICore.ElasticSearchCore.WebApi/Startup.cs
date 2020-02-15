using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QICore.ElasticSearchCore.WebApi.Common;
using QICore.ElasticSearchCore.WebApi.Dao;
using QICore.ElasticSearchCore.WebApi.OptionModel;
using System;
using System.Reflection;

namespace QICore.ElasticSearchCore.WebApi
{
    /// <summary>
    /// Startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> 
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// IConfiguration 配置.
        /// </summary>
        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }
        /// <summary>
        /// 配置服务.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
          
            #region Autofac

            //返回的void 修改为 IServiceProvider 这是为了让第三方Ioc容易接管通道 具体在第几层怎么实现我没有深入研究  

            //var builder = new ContainerBuilder();//实例化 AutoFac  容器            
            //builder.Populate(services);//管道寄居
            //builder.RegisterModule<Base>();//使用Module 重写的方式配置 就不需要每次都来修改Startup文件了。后期打算改成json的。
            ////builder.RegisterType<AutofaceTest.Service.Service.UserService>().As<Service.Interface.IUserService>();//UserService注入到IUserService
            //ApplicationContainer = builder.Build();//IUserService UserService 构造

            //return new AutofacServiceProvider(ApplicationContainer);//将autofac反馈到管道中

            //var builder = new ContainerBuilder();//实例化 AutoFac  容器 
            //Type basetype = typeof(IBase);
            //builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            //    .Where(t => basetype.IsAssignableFrom(t) && t.IsClass)
            //    .AsImplementedInterfaces().InstancePerLifetimeScope();//Autofac注入有多种不同的生命周期类型，分别为InstancePerLifetimeScope、SingleInstance、InstancePerDependency等，各位在项目中按需选择即可。

            //var _container = builder.Build();

            #endregion
            services.AddOptions(Configuration);//读取配置文件           
            services.AddControllers();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">IApplicationBuilder.</param>
        /// <param name="env">IWebHostEnvironment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //添加依赖注入关系
            builder.RegisterModule(new AutofacModuleRegister());
            var controllerBaseType = typeof(ControllerBase);
            //在控制器中使用依赖注入
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
                .PropertiesAutowired();
        }
    }
    
}
