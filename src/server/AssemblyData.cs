using Web.Utils;

namespace Web
{
    public class AssemblyData : BaseAssemblyData<AssemblyData>
    {
        public static readonly IAssemblyData Instance = new AssemblyData();
    }
}