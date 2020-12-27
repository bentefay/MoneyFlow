using System;
using System.Collections.Generic;
using System.Reflection;

namespace Web.Utils
{
    public abstract class BaseAssemblyData<T> : IAssemblyData
    {
        public Assembly Assembly { get; }
        public Lazy<IReadOnlyList<Type>> Types { get; }

        protected BaseAssemblyData()
        {
            Assembly = typeof(T).Assembly;
            Types = new Lazy<IReadOnlyList<Type>>(Assembly.GetTypes());
        }
    }

    public interface IAssemblyData
    {
        Assembly Assembly { get; }
        Lazy<IReadOnlyList<Type>> Types { get; }
    }
}