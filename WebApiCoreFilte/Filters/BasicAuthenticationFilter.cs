using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreFilte.Identities;

namespace WebApiCoreFilte.Filters
{
    public class BasicAuthenticationFilter : IAuthorizationFilter
    {
        private IConfiguration _configuration;
        public BasicAuthenticationFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string authHeader = context.HttpContext.Request.Headers["Authorization"];

            if (authHeader == null || !authHeader.StartsWith("Basic "))
            {
                SetUnauthorizedResult(context);
            }
            else
            {
                try
                {
                    BasicAuthenticationIdentity identity = GetIdentity(authHeader);
                    if (Autherize(identity.Name, identity.Password))
                    {
                        var principal = new GenericPrincipal(identity, null);
                        Thread.CurrentPrincipal = principal;
                        context.HttpContext.User = principal;
                        return;
                    }
                    else
                    {
                        SetUnauthorizedResult(context);
                    }
                }
                catch
                {
                    SetUnauthorizedResult(context);
                }
            }
        }

        private bool Autherize(string Username, string Password)
        {
            if (Username == _configuration["BasicAuth:Username"] && Password == _configuration["BasicAuth:Passsword"])
            {
                return true;
            }
            return false;
        }

        private BasicAuthenticationIdentity GetIdentity(string AuthHeader)
        {
            AuthHeader = AuthHeader.Replace("Basic ", "").Trim();
            string[] credentials = Encoding.UTF8.GetString(Convert.FromBase64String(AuthHeader)).Split(':');
            string username = credentials[0];
            string password = credentials[1];
            return new BasicAuthenticationIdentity(username, password);
        }


        private AuthorizationFilterContext SetUnauthorizedResult(AuthorizationFilterContext context)
        {
            var host = context.HttpContext.Request.Host.Host;
            context.HttpContext.Response.Headers["WWW-Authenticate"] += string.Format("Basic realm=\"{0}\"", host);
            context.Result = new UnauthorizedResult();
            return context;
        }
    }
}

