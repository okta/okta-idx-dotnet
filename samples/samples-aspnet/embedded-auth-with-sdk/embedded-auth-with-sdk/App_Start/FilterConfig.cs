using System.Web;
using System.Web.Mvc;

namespace embedded_auth_with_sdk
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
