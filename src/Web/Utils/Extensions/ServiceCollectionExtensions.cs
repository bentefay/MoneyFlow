using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Web.Utils.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection InDevelopment(this IServiceCollection @this, IWebHostEnvironment env, Func<IServiceCollection, IServiceCollection> f)
        {
            return env.IsDevelopment() ? f(@this) : @this;
        }
        
        public static IServiceCollection NotInDevelopment(this IServiceCollection @this, IWebHostEnvironment env, Func<IServiceCollection, IServiceCollection> f)
        {
            return !env.IsDevelopment() ? f(@this) : @this;
        }
    }
}