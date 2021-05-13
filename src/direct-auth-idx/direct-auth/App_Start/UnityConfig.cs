using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;
using Unity;
using Unity.Injection;
using Unity.Mvc5;

namespace direct_auth
{
    using Okta.Idx.Sdk;

    using Unity.Lifetime;

    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();


            container.RegisterFactory<IIdxClient>(o => new IdxClient(), new TransientLifetimeManager());
            container.RegisterFactory<IAuthenticationManager>(o => HttpContext.Current.GetOwinContext().Authentication, new SingletonLifetimeManager());

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}