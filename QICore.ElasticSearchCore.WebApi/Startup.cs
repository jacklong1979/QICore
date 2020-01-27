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
        /// IConfiguration ����.
        /// </summary>
        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }
        /// <summary>
        /// ���÷���.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
          
            #region Autofac

            //���ص�void �޸�Ϊ IServiceProvider ����Ϊ���õ�����Ioc���׽ӹ�ͨ�� �����ڵڼ�����ôʵ����û�������о�  

            //var builder = new ContainerBuilder();//ʵ���� AutoFac  ����            
            //builder.Populate(services);//�ܵ��ľ�
            //builder.RegisterModule<Base>();//ʹ��Module ��д�ķ�ʽ���� �Ͳ���Ҫÿ�ζ����޸�Startup�ļ��ˡ����ڴ���ĳ�json�ġ�
            ////builder.RegisterType<AutofaceTest.Service.Service.UserService>().As<Service.Interface.IUserService>();//UserServiceע�뵽IUserService
            //ApplicationContainer = builder.Build();//IUserService UserService ����

            //return new AutofacServiceProvider(ApplicationContainer);//��autofac�������ܵ���

            //var builder = new ContainerBuilder();//ʵ���� AutoFac  ���� 
            //Type basetype = typeof(IBase);
            //builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            //    .Where(t => basetype.IsAssignableFrom(t) && t.IsClass)
            //    .AsImplementedInterfaces().InstancePerLifetimeScope();//Autofacע���ж��ֲ�ͬ�������������ͣ��ֱ�ΪInstancePerLifetimeScope��SingleInstance��InstancePerDependency�ȣ���λ����Ŀ�а���ѡ�񼴿ɡ�

            //var _container = builder.Build();

            #endregion
            services.AddOptions(Configuration);//��ȡ�����ļ�           
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
            //�������ע���ϵ
            builder.RegisterModule(new AutofacModuleRegister());
            var controllerBaseType = typeof(ControllerBase);
            //�ڿ�������ʹ������ע��
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
                .PropertiesAutowired();
        }
    }
    
}
