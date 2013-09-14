﻿using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using CodeCamp.Domain;
using CodeCamp.Domain.Infrastructure;
using CodeCamp.Infrastructure.Authentication;
using CodeCamp.Infrastructure.Data;
using CodeCamp.Infrastructure.Logging;
using CodeCamp.Infrastructure.Views;
using Raven.Client;
using SimpleAuthentication.Mvc;

namespace CodeCamp.Infrastructure.IoC {
    public class ContainerConfig {
        public static void Configure() {
            var builder = new ContainerBuilder();

            RegisterMVCComponents(builder);
            RegisterApplicationComponents(builder);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));    
        }

        static void RegisterApplicationComponents(ContainerBuilder builder) {
            builder.RegisterModule<NLogModule>();
            
            builder.RegisterType<ViewInfo>().InstancePerHttpRequest();
            
            builder.RegisterType<SingleWebServerApplicationState>().As<IApplicationState>().InstancePerHttpRequest();
            builder.RegisterType<DefaultApplicationBus>().As<IApplicationBus>().InstancePerHttpRequest();
            
            builder.Register(x => RavenDBConfig.CreateDocumentStore()).As<IDocumentStore>().SingleInstance();
            builder.Register(x => x.Resolve<IDocumentStore>().OpenSession()).As<IDocumentSession>().InstancePerHttpRequest();
            
            builder.RegisterType<AuthenticationCallbackProvider>().As<IAuthenticationCallbackProvider>();
            builder.RegisterType<SimpleAuthenticationController>().As<SimpleAuthenticationController>().InstancePerHttpRequest();
            
            builder.RegisterType<DefaultSecurityEncoder>().As<ISecurityEncoder>().SingleInstance();
            builder.RegisterType<DefaultSlugConverter>().As<ISlugConverter>().SingleInstance();
        }

        static void RegisterMVCComponents(ContainerBuilder builder) {
            //Register Controllers
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            //Register Model Binders
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();

            //Register Standard HTTP Abstractions
            builder.RegisterModule(new AutofacWebTypesModule());

            //Enable property injection for Views
            builder.RegisterSource(new ViewRegistrationSource());

            //Enable property injection for Filters
            builder.RegisterFilterProvider();
        }
    }
}