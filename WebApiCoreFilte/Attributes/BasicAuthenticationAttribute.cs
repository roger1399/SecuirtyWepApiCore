using Microsoft.AspNetCore.Mvc;
using System;
using WebApiCoreFilte.Filters;

namespace WebApiCoreFilte.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthenticationAttribute : TypeFilterAttribute
    {
        public BasicAuthenticationAttribute()
            : base(typeof(BasicAuthenticationFilter))
        {
        }
    }
}
