using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Web.Utils.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder InDevelopment(this IApplicationBuilder @this, IWebHostEnvironment env, Func<IApplicationBuilder, IApplicationBuilder> f)
        {
            return env.IsDevelopment() ? f(@this) : @this;
        }

        public static IApplicationBuilder NotInDevelopment(this IApplicationBuilder @this, IWebHostEnvironment env, Func<IApplicationBuilder, IApplicationBuilder> f)
        {
            return !env.IsDevelopment() ? f(@this) : @this;
        }
    }
}