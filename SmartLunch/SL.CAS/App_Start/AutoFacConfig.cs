using System;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;

namespace SL.CAS
{
    /// <summary>
    /// Autofac依赖注入和控制反转配置
    /// </summary>
    public class AutoFacConfig
    {
        /// <summary>
        /// 注册类库
        /// </summary>
        public static void RegisterAutofac()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            Assembly dal = Assembly.Load("SL.SqlServerDal");
            Type[] rtypes = dal.GetTypes();
            builder.RegisterTypes(rtypes).AsImplementedInterfaces();
            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}