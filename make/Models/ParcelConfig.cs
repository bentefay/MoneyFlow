using System.Linq;

namespace Make.Models
{
    public class ParcelConfig
    {
        // 0 (Disabled) | 1 (Errors) | 2 (Errors + Warnings) | 3 (Everything)
        public int Verbosity = 2;

        public ParcelProject Project =>
            new[] {"Client"}
                .Select(name => new ParcelProject(name, $"src/{name}"))
                .First();
    }
}