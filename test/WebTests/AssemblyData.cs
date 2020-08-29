using System.Collections.Generic;
using Web.Utils;

namespace WebTests
{
    public static class Assemblies
    {
        public static readonly IReadOnlyList<IAssemblyData> All = new[]
        {
            Web.AssemblyData.Instance
        };
    }
}